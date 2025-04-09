//Chon ghe 

document.addEventListener('DOMContentLoaded', function () {
    const seats = document.querySelectorAll('.seat:not(.unavailable)');
    let selectedSeat = null;

    seats.forEach(seat => {
        seat.addEventListener('click', function () {
            if (selectedSeat) {
                selectedSeat.classList.remove('selected');
            }
            this.classList.add('selected');
            selectedSeat = this;
            document.querySelector('input[name="selectedSeatId"]').value = this.dataset.seatId;
        });
    });
});

document.getElementById("submitSeatBtn").addEventListener("click", function (e) {
    const selectedSeatId = document.querySelector('input[name="selectedSeatId"]').value;
    if (!selectedSeatId) {
        e.preventDefault(); // Chặn gửi form nếu chưa chọn ghế
        alert("Vui lòng chọn một ghế trước khi xác nhận.");
        return;
    }

    // Cho phép submit, nhưng đổi nội dung nút
    this.innerHTML = "Đang xử lý...";
    this.style.backgroundColor = "#aaa";
    // Không disable để form submit bình thường
});