let isChatOpen = false;
function getSessionId() {
    let sessionId = sessionStorage.getItem("sessionId");
    if (!sessionId) {
        sessionId = uuid.v4();
        sessionStorage.setItem("sessionId", sessionId);
    }
    return sessionId;
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

    // Hiển thị đang nhập...
    showTypingIndicator();
    // Gửi tới API chatbot
    fetch(API_URL_Chatbot, {
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
            }, 500);
        });
}
document.getElementById("chat-button").addEventListener("click", toggleChat);
document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("chat-container");
    container.style.opacity = "0";
    container.style.transform = "translateY(20px)";

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

