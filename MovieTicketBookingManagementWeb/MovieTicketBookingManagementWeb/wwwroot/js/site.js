
    document.addEventListener('DOMContentLoaded', function () {
    const reviewForm = document.getElementById('reviewForm');

    // Gửi hoặc sửa đánh giá
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
    body: JSON.stringify({movieId, rating, comment, reviewId})
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

    // Mở form sửa
    document.addEventListener('click', function (e) {
        const link = e.target.closest('.edit-review-link');
    if (link) {
        reviewForm.dataset.editingId = link.dataset.id;
    document.querySelector('#Rating').value = link.dataset.rating;
    document.querySelector('#Comment').value = link.dataset.comment;
    link.closest('.review-item').scrollIntoView({behavior: 'smooth' });
        }
    });

    // Xóa bình luận
    document.addEventListener('click', function (e) {
        const link = e.target.closest('.delete-review-link');
    if (link && confirm("Bạn chắc chắn muốn xóa?")) {
        fetch(`/Movies/DeleteReview/${link.dataset.id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value,
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    updateReviewList(data.reviews);
                }
            });
        }
    });

    // Cập nhật danh sách đánh giá
    function updateReviewList(reviews) {
        const reviewList = document.getElementById('reviewList');
    reviewList.innerHTML = '';
    let total = 0;

        reviews.forEach(r => {
        total += r.rating;
    reviewList.innerHTML += `
    <div class="review-item border-bottom py-2" data-review-id="${r.id}">
        <p class="mb-1">
            <strong>${r.userName}</strong> ⭐${r.rating}
            <span class="text-muted small">• ${r.reviewTime}</span>
            ${r.canEditOrDelete ? `
                        <span class="float-end">
                            ${r.isOwner ? `<a href="#" class="text-primary edit-review-link me-2" data-id="${r.id}" data-rating="${r.rating}" data-comment="${r.comment}">
                                <i class="bi bi-pencil-square"></i> Sửa</a>` : ''}
                            <a href="#" class="text-danger delete-review-link" data-id="${r.id}">
                                <i class="bi bi-trash-fill"></i> Xóa</a>
                        </span>` : ''}
        </p>
        <p class="mb-0">${r.comment}</p>
    </div>`;
        });

    // Cập nhật điểm trung bình
    const avg = (total / reviews.length).toFixed(1);
    const display = document.getElementById('avgRatingDisplay');
    display.classList.remove('text-muted');
    display.classList.add('text-warning');
    display.innerHTML = `⭐ ${avg} / 5.0 (${reviews.length} lượt đánh giá)`;
    }
});

// Dark mode toggle
document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('darkModeToggle');
    if (toggleBtn) {
        toggleBtn.addEventListener('click', function () {
            document.body.classList.toggle('dark-mode');
            localStorage.setItem('darkMode', document.body.classList.contains('dark-mode'));
        });

        if (localStorage.getItem('darkMode') === 'true') {
            document.body.classList.add('dark-mode');
        }
    }
});

