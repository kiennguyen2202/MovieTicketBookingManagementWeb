document.addEventListener('DOMContentLoaded', () => {
    initShowtimeSelector();
    initReviewForm();
    initDarkModeToggle();
});

// ==============================
// 🎬 Suất chiếu theo ngày
// ==============================
function initShowtimeSelector() {
    const dateItems = document.querySelectorAll('.date-item');
    const showtimeSlots = document.getElementById('showtimeSlots');
    const movieId = document.querySelector('input[name="movieId"]').value;

    function loadShowtimesByDate(date) {
        fetch(`/Customer/Movies/GetShowtimesByDate?movieId=${movieId}&date=${date}`)
            .then(res => res.json())
            .then(data => {
                showtimeSlots.innerHTML = '';
                if (!data || data.length === 0) {
                    showtimeSlots.innerHTML = '<p class="text-muted text-center">Không có suất chiếu nào trong ngày này.</p>';
                    return;
                }

                const table = document.createElement('table');
                table.className = 'table table-striped table-hover text-center shadow rounded-3';
                table.innerHTML = `
                    <thead class="table-dark">
                        <tr>
                            <th>🕓 Giờ</th>
                            <th>🏠 Phòng</th>
                            <th>🎬 Rạp</th>
                            <th>🎟</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${data.map(st => `
                            <tr>
                                <td>${st.startTime}</td>
                                <td>${st.roomName}</td>
                                <td><strong>${st.cinemaName}</strong></td>
                                <td>
                                    <a href="/Customer/Tickets/SelectSeats?showtimeId=${st.id}" class="btn btn-success btn-sm shadow">
                                        <i class="bi bi-ticket-perforated-fill"></i> Đặt vé
                                    </a>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                `;
                showtimeSlots.appendChild(table);
            });
    }

    // Chọn ngày đầu tiên mặc định
    if (dateItems.length > 0) {
        dateItems[0].classList.add('active');
        loadShowtimesByDate(dateItems[0].dataset.date);
    }

    // Click chọn ngày
    dateItems.forEach(item => {
        item.addEventListener('click', function () {
            dateItems.forEach(i => i.classList.remove('active'));
            this.classList.add('active');
            loadShowtimesByDate(this.dataset.date);
        });
    });
}

// ==============================
// ⭐ Đánh giá phim
// ==============================
function initReviewForm() {
    const reviewForm = document.getElementById('reviewForm');
    const reviewList = document.getElementById('reviewList');

    if (!reviewForm) return;

    reviewForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const movieId = reviewForm.querySelector('input[name="movieId"]').value;
        const rating = reviewForm.querySelector('select[name="rating"]').value;
        const comment = reviewForm.querySelector('textarea[name="comment"]').value;
        const reviewId = reviewForm.dataset.editingId;

        fetch(reviewId ? '/Movies/EditReview' : '/Movies/AddReview', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token,
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify({ movieId, rating, comment, reviewId })
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    updateReviewList(data.reviews);
                    reviewForm.reset();
                    delete reviewForm.dataset.editingId;
                }
            });
    });

    

    // Cập nhật danh sách
    function updateReviewList(reviews) {
        reviewList.innerHTML = '';
        let total = 0;

        reviews.forEach(r => {
            total += r.rating;
            reviewList.innerHTML += `
                <div class="review-item border-bottom py-2" data-review-id="${r.id}">
                    <p class="mb-1">
                        <strong>${r.userName}</strong> ⭐${r.rating}
                        <span class="text-muted small">• ${r.reviewTime}</span>
                        
                    </p>
                    <p class="mb-0">${r.comment}</p>
                </div>
            `;
        });

        // Trung bình đánh giá
        const avg = (total / reviews.length).toFixed(1);
        const display = document.getElementById('avgRatingDisplay');
        if (display) {
            display.classList.remove('text-muted');
            display.classList.add('text-warning');
            display.innerHTML = `⭐ ${avg} / 5.0 (${reviews.length} lượt đánh giá)`;
        }
    }
}

// ==============================
// 🌙 Dark Mode Toggle
// ==============================
function initDarkModeToggle() {
    const toggleBtn = document.getElementById('darkModeToggle');
    if (!toggleBtn) return;

    toggleBtn.addEventListener('click', () => {
        document.body.classList.toggle('dark-mode');
        localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
    });

    if (localStorage.getItem('darkMode') === 'true') {
        document.body.classList.add('dark-mode');
    }
}
