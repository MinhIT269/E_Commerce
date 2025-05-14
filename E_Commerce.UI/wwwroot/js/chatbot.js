// Thay PHP config bằng Razor: đặt URL API trong ViewData hoặc appsettings
const API_URL = '@(Configuration["Chatbot:ApiUrl"])'; // hoặc hard-code

let isChatOpen = false;

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
    fetch(API_URL, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        credentials: "include",
        body: JSON.stringify({ query: message })
    })
        .then(response => response.json())
        .then(data => {
            // Đợi một chút để tạo hiệu ứng đang nhập
            setTimeout(() => {
                hideTypingIndicator();

                const botMsg = document.createElement("div");
                botMsg.className = "bot-message message";

                // Xử lý tin nhắn từ API
                const responseText = data.response || "Xin lỗi, tôi không thể trả lời lúc này.";
                botMsg.textContent = responseText;

                chatBody.appendChild(botMsg);
                scrollToBottom();
            }, 600 + Math.random() * 800); // Thời gian ngẫu nhiên để tạo cảm giác tự nhiên
        })
        .catch(error => {
            setTimeout(() => {
                hideTypingIndicator();

                const errorMsg = document.createElement("div");
                errorMsg.className = "bot-message message";
                errorMsg.textContent = "❌ Xin lỗi, đã xảy ra lỗi kết nối. Vui lòng thử lại sau.";

                chatBody.appendChild(errorMsg);
                scrollToBottom();
                console.error("API Error:", error);
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

