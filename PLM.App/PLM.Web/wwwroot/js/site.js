const ParkingModule = (() => {
    const checkInBtn = document.getElementById('checkInBtn');
    const checkOutBtn = document.getElementById('checkOutBtn');
    const tagNumberInput = document.getElementById('tagNumber');
    const parkingSnapshot = document.getElementById('parkingSnapshot');

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
            .then(updateParkingSnapshot)
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
                if (!data.isSuccess) {
                    document.getElementById('errorMessage').innerHTML = `<strong>Error:</strong> ${message}`;
                    document.getElementById('errorMessage').style.display = 'block';
                }
            })
            .then(updateParkingSnapshot)
            .catch(error => console.error('Error:', error));
    };

    const handleResponse = (response, errorMessage) => {
        if (!response.ok) {
            throw new Error(errorMessage);
        }
        return response.json();
    };

    const updateParkingSnapshot = (data) => {
        fetch('/parking-snapshot')
            .then(response => response.text())
            .then(html => {
                parkingSnapshot.innerHTML = html;
            })
            .catch(error => console.error('Error:', error));
    };

    const showError = (message) => {
        // Update the HTML content with the error message
        document.getElementById('errorMessage').innerHTML = `<p>Error: ${message}</p>`;
    }


    return {
        init: () => {
            updateParkingSnapshot();
            checkInBtn.addEventListener('click', checkIn);
            checkOutBtn.addEventListener('click', checkOut);
        }
    };
})();

document.addEventListener('DOMContentLoaded', ParkingModule.init);
