const API_URL = '@(Configuration["Chatbot:ApiUrl"])';
let isChatOpen = false;

function getSessionId() {
    let sessionId = sessionStorage.getItem("sessionId");
    if (!sessionId) {
        sessionId = uuid.v4();
        sessionStorage.setItem("sessionId", sessionId);
    }
    return sessionId;
}

// Thêm functions để quản lý lịch sử chat
function saveChatHistory() {
    const chatBody = document.getElementById("chat-body");
    const messages = [];

    // Lấy tất cả các tin nhắn (trừ typing indicator)
    const messageElements = chatBody.querySelectorAll('.message:not(#typing-indicator)');

    messageElements.forEach(element => {
        const isUser = element.classList.contains('user-message');
        const content = isUser ? element.textContent : element.innerHTML;

        messages.push({
            type: isUser ? 'user' : 'bot',
            content: content,
            timestamp: Date.now()
        });
    });

    const sessionId = getSessionId();
    const historyKey = `chatHistory_${sessionId}`;

    console.log('Saving chat history:', historyKey, messages);
    sessionStorage.setItem(historyKey, JSON.stringify(messages));
}

function loadChatHistory() {
    const sessionId = getSessionId();
    const chatHistory = sessionStorage.getItem(`chatHistory_${sessionId}`);

    console.log('Loading chat history for session:', sessionId);
    console.log('Chat history data:', chatHistory);

    if (!chatHistory) {
        console.log('No chat history found');
        return;
    }

    const messages = JSON.parse(chatHistory);
    console.log('Parsed messages:', messages);
    const chatBody = document.getElementById("chat-body");

    // Xóa nội dung hiện tại (trừ typing indicator)
    const existingMessages = chatBody.querySelectorAll('.message:not(#typing-indicator)');
    existingMessages.forEach(msg => msg.remove());

    // Thêm lại các tin nhắn từ lịch sử
    messages.forEach(message => {
        const msgElement = document.createElement("div");
        msgElement.className = `${message.type}-message message`;

        if (message.type === 'user') {
            msgElement.textContent = message.content;
        } else {
            msgElement.innerHTML = message.content;

            // Áp dụng lại syntax highlighting nếu có
            if (typeof hljs !== 'undefined') {
                const codeBlocks = msgElement.querySelectorAll('pre code');
                codeBlocks.forEach(block => {
                    hljs.highlightElement(block);
                });
            }
        }

        chatBody.appendChild(msgElement);
    });

    scrollToBottom();
}

function clearChatHistory() {
    sessionStorage.removeItem(`chatHistory_${getSessionId()}`);
    const chatBody = document.getElementById("chat-body");
    const existingMessages = chatBody.querySelectorAll('.message:not(#typing-indicator)');
    existingMessages.forEach(msg => msg.remove());
}

function toggleChat() {
    const container = document.getElementById("chat-container");
    isChatOpen = !isChatOpen;

    if (isChatOpen) {
        container.style.display = "flex";

        // Animation after display is set
        setTimeout(() => {
            container.style.opacity = "1";
            container.style.transform = "translateY(0)";
            document.getElementById("user-input").focus();

            // Load lịch sử chat sau khi animation hoàn thành
            console.log('Chat opened, loading history...');
            loadChatHistory();
        }, 10);
    } else {
        container.style.opacity = "0";
        container.style.transform = "translateY(20px)";
        setTimeout(() => {
            container.style.display = "none";
        }, 300);
    }
}

function handleKey(event) {
    if (event.key === "Enter") {
        sendMessage();
    }
}

function showTypingIndicator() {
    const chatBody = document.getElementById("chat-body");
    const indicator = document.getElementById("typing-indicator");

    // Đảm bảo indicator được thêm vào cuối cùng
    chatBody.appendChild(indicator);
    indicator.style.display = "flex";
    scrollToBottom();
}

function hideTypingIndicator() {
    const indicator = document.getElementById("typing-indicator");
    indicator.style.display = "none";
}

function scrollToBottom() {
    const chatBody = document.getElementById("chat-body");
    chatBody.scrollTop = chatBody.scrollHeight;
}

// Cấu hình Marked để cho phép hiển thị HTML (nếu cần)
marked.setOptions({
    breaks: true, // Tự động thêm line breaks
    sanitize: false, // Cho phép HTML trong Markdown
    highlight: function (code, lang) {
        // Nếu có highlight.js
        if (typeof hljs !== 'undefined') {
            const language = hljs.getLanguage(lang) ? lang : 'plaintext';
            return hljs.highlight(code, { language }).value;
        }
        return code;
    }
});

function sendMessage() {
    const input = document.getElementById("user-input");
    const message = input.value.trim();
    if (message === "") return;

    const chatBody = document.getElementById("chat-body");

    // Hiển thị tin nhắn người dùng
    const userMsg = document.createElement("div");
    userMsg.className = "user-message message";
    userMsg.textContent = message;
    chatBody.appendChild(userMsg);

    input.value = "";
    scrollToBottom();

    // Lưu lịch sử sau khi thêm tin nhắn user
    saveChatHistory();

    // Hiển thị đang nhập...
    showTypingIndicator();
    // Gửi tới API chatbot
    fetch("http://localhost:5678/webhook/905a2477-34b8-4b91-8906-e1661f3286ea", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            prompt: message,
            sessionId: getSessionId()
        })
    })
        .then(response => response.json())
        .then(data => {
            // Đợi một chút để tạo hiệu ứng đang nhập
            setTimeout(() => {
                hideTypingIndicator();

                const botMsg = document.createElement("div");
                botMsg.className = "bot-message message";

                // Xử lý tin nhắn từ API và chuyển đổi Markdown thành HTML
                const responseText = data.message || "Xin lỗi, tôi không thể trả lời lúc này.";

                // Sử dụng marked để chuyển Markdown thành HTML
                botMsg.innerHTML = marked.parse(responseText);

                // Nếu có highlight.js, áp dụng syntax highlighting cho code blocks
                if (typeof hljs !== 'undefined') {
                    const codeBlocks = botMsg.querySelectorAll('pre code');
                    codeBlocks.forEach(block => {
                        hljs.highlightElement(block);
                    });
                }

                chatBody.appendChild(botMsg);
                scrollToBottom();

                // Lưu lịch sử sau khi thêm tin nhắn bot
                saveChatHistory();
            }, 600 + Math.random() * 800);
        })
        .catch(error => {
            setTimeout(() => {
                hideTypingIndicator();

                const errorMsg = document.createElement("div");
                errorMsg.className = "bot-message message";
                errorMsg.textContent = "❌ Xin lỗi, đã xảy ra lỗi kết nối. Vui lòng thử lại sau.";

                chatBody.appendChild(errorMsg);
                scrollToBottom();

                // Lưu lịch sử sau khi thêm tin nhắn lỗi
                saveChatHistory();
            }, 500);
        });
}

document.getElementById("chat-button").addEventListener("click", toggleChat);

document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("chat-container");
    container.style.opacity = "0";
    container.style.transform = "translateY(20px)";

    // Load lịch sử chat ngay khi trang được tải (không cần check isChatOpen)
    console.log('DOM loaded, loading chat history...');
    loadChatHistory();

    document.addEventListener("click", (event) => {
        const container = document.getElementById("chat-container");
        const button = document.getElementById("chat-button");

        if (isChatOpen &&
            !container.contains(event.target) &&
            !button.contains(event.target)) {
            toggleChat();
        }
    });
});