const ParkingModule = (() => {
    const checkInBtn = document.getElementById('checkInBtn');
    const checkOutBtn = document.getElementById('checkOutBtn');
    const tagNumberInput = document.getElementById('tagNumber');
    const parkingSnapshot = document.getElementById('parkingSnapshot');
    const totalChargedAmount = document.getElementById("totalChargedAmount");
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');
    const loadStatsButton = document.getElementById('loadStatsButton');
    const statisticsModal = document.getElementById('statisticsModal');
    const statisticsModalBody = document.getElementById('statisticsModalBody');

    const checkIn = () => {
        const tagNumber = tagNumberInput.value;
        if (!validateTagNumber(tagNumber)) return;

        fetch('/checkin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ tagNumber: tagNumber.trim() })
        })
            .then(response => handleResponse(response, 'Failed to check in car'))
            .then(data => {
                showMessage(data);
                if (data.isSuccess) {
                    tagNumberInput.value = '';
                    updateParkingSnapshot();
                }
            })
            .catch(error => console.error('Error:', error));
    };

    const checkOut = () => {
        const tagNumber = tagNumberInput.value;

        if (!validateTagNumber(tagNumber)) return;

        fetch('/checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ tagNumber: tagNumber.trim() })
        })
            .then(response => handleResponse(response, 'Failed to check out car'))
            .then(data => {
                showMessage(data);
                if (data.isSuccess) {
                    tagNumberInput.value = '';
                    totalChargedAmount.textContent = `$${data?.model?.toFixed(2)}`;
                    updateParkingSnapshot();
                }
            })
            .then(updateParkingSnapshot)
            .catch(error => console.error('Error:', error));
    };

    const validateTagNumber = (value) => {
        if (value.trim() === "") {
            errorMessage.textContent = "";
            showMessage({ isSuccess: false, message: "Tag number is required." })
            return false;
        }

        return true;
    }

    const updateParkingSnapshot = (data) => {
        fetch('/parking-snapshot')
            .then(response => response.text())
            .then(html => {
                parkingSnapshot.innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    };

    const showParkingStatistics = () => {
        fetch('/parking-statistics')
            .then(response => response.text())
            .then(data => {
                statisticsModalBody.innerHTML = data;
                $(statisticsModal).modal('show');
            })
            .catch(error => {
                console.error('Error fetching status:', error);
            });
    };

    const handleResponse = (response, errorMessage) => {
        if (!response.ok) {
            throw new Error(errorMessage);
        }
        return response.json();
    };

    const showMessage = (data) => {
        const element = data?.isSuccess ? successMessage : errorMessage;
        element.innerHTML = `<strong>${data?.isSuccess ? 'Success' : 'Error'}:</strong> ${data.message}`;
        element.style.display = 'block';
        setTimeout(function () {
            element.style.display = 'none';
        }, 6000);
    };

    return {
        init: () => {
            updateParkingSnapshot();
            checkInBtn.addEventListener('click', checkIn);
            checkOutBtn.addEventListener('click', checkOut);
            loadStatsButton.addEventListener('click', showParkingStatistics);
        }
    };
})();

document.addEventListener('DOMContentLoaded', ParkingModule.init);