//Chon bap nuoc  

document.addEventListener('DOMContentLoaded', function () {
    const minusButtons = document.querySelectorAll('.minus');
    const plusButtons = document.querySelectorAll('.plus');

    const updateState = (id, quantity) => {
        const idInput = document.querySelector(`.id-input[data-id="${id}"]`);
        const qtyInput = document.querySelector(`.qty-input[data-id="${id}"]`);
        const display = document.querySelector(`.quantity-display[data-id="${id}"]`);

        if (!qtyInput || !idInput || !display) return;

        qtyInput.value = quantity;
        display.textContent = quantity;

        if (quantity > 0) {
            qtyInput.disabled = false;
            idInput.disabled = false;
        } else {
            qtyInput.disabled = true;
            idInput.disabled = true;
        }
    };

    minusButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.dataset.id;
            const display = document.querySelector(`.quantity-display[data-id="${id}"]`);
            let quantity = parseInt(display.textContent);
            if (quantity > 0) quantity--;
            updateState(id, quantity);
        });
    });

    plusButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.dataset.id;
            const display = document.querySelector(`.quantity-display[data-id="${id}"]`);
            let quantity = parseInt(display.textContent);
            quantity++;
            updateState(id, quantity);
        });
    });
});


function prepareComboForm() {
    document.querySelectorAll('.qty-input, .id-input').forEach(input => {
        const dataId = input.dataset.id;
        const qtyInput = document.querySelector(`.qty-input[data-id="${dataId}"]`);
        const idInput = document.querySelector(`.id-input[data-id="${dataId}"]`);
        const qty = parseInt(qtyInput.value);

        if (qty > 0) {
            qtyInput.disabled = false;
            idInput.disabled = false;
        } else {
            qtyInput.disabled = true;
            idInput.disabled = true;
        }
    });

    const btn = document.getElementById("submitBtn");
    btn.disabled = true;
    btn.innerHTML = "Đang xử lý...";

    return true;
}


document.getElementById("combo-form").addEventListener("submit", function () {
    const btn = document.getElementById("submitBtn");
    btn.disabled = true;
    btn.innerHTML = "Đang xử lý...";
});