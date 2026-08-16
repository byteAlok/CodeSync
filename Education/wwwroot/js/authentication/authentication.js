
const mainBox = document.getElementById("mainBox");
document.querySelector(".register-btn").addEventListener("click", () => {
    mainBox.classList.add("active");
});
document.querySelector(".login-btn").addEventListener("click", () => {
    mainBox.classList.remove("active");
});