const ParkingModule = (() => {
    const checkInBtn = document.getElementById('checkInBtn');
    const checkOutBtn = document.getElementById('checkOutBtn');
    const tagNumberInput = document.getElementById('tagNumber');
    const parkingSnapshot = document.getElementById('parkingSnapshot');

    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');
    const statisticsModal = document.getElementById('statisticsModal');
    const statisticsModalBody = document.getElementById('statisticsModalBody');

    const checkIn = () => {
        const tagNumber = tagNumberInput.value;
        fetch('/checkin', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ tagNumber })
        })
            .then(response => handleResponse(response, 'Failed to check in car'))
            .then(data => {
                showMessage(data);
                if (data.isSuccess) {
                    updateParkingSnapshot();
                }
            })
            .catch(error => console.error('Error:', error));
    };

    const checkOut = () => {
        const tagNumber = tagNumberInput.value;
        fetch('/checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ tagNumber })
        })
            .then(response => handleResponse(response, 'Failed to check out car'))
            .then(data => {
                showMessage(data);
                if (data.isSuccess) {
                    updateParkingSnapshot();
                }
            })
            .then(updateParkingSnapshot)
            .catch(error => console.error('Error:', error));
    };

    const updateParkingSnapshot = (data) => {
        fetch('/parking-snapshot')
            .then(response => response.text())
            .then(html => {
                parkingSnapshot.innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    };

    const showParkingStatistics = (data) => {
        fetch('/parking-statistics')
            .then(response => response.text())
            .then(data => {
                modalBody.innerHTML = data;
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
        }, 5000);
    };


    return {
        init: () => {
            updateParkingSnapshot();
            checkInBtn.addEventListener('click', checkIn);
            checkOutBtn.addEventListener('click', checkOut);
            document.getElementById('loadStatsButton').addEventListener('click', function () {
                fetch('/parking-statistics')
                    .then(response => {
                        if (response.ok) {
                            return response.text();
                        } else {
                            throw new Error('Failed to fetch statistics');
                        }
                    })
                    .then(data => {
                        statisticsModalBody.innerHTML = data;
                        $(statisticsModal).modal('show');
                    })
                    .catch(error => console.error('Error:', error));
            });
        }
    };
})();

document.addEventListener('DOMContentLoaded', ParkingModule.init);