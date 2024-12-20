

document.getElementById('maar').addEventListener('click', () => {
    const markAsReadButton = document.getElementById('maar');

    // Disable the button to prevent multiple clicks
    markAsReadButton.disabled = true;

    // Send request to server to mark all notifications as read
    fetch('http://localhost:5293/api/AccountAPI/markAllAsRead', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update UI to reflect all notifications as read
                const unreadNotifications = document.querySelectorAll('.notification.unread');
                unreadNotifications.forEach(notification => {
                    notification.classList.remove('unread');
                    notification.classList.add('read'); // Optional: Add "read" class for styling
                    const redDot = notification.querySelector('#red-dot');
                    if (redDot) redDot.style.display = 'none'; // Hide the red dot
                });

                // Reset unread count
                const unreadCountElement = document.getElementById('unread-count');
                const unreadCountElement1 = document.getElementById('unread-count1');
                unreadCountElement.textContent = '0';
                unreadCountElement1.textContent = '0';

                console.log(data.message); // Display success message in console
            } else {
                console.error(data.message); // Display error message in console
            }
        })
        .catch(error => {
            console.error('Error marking notifications as read:', error);
        })
        .finally(() => {
            // Re-enable the button after 3 seconds
            setTimeout(() => {
                markAsReadButton.disabled = false;
            }, 3000);
        });
});
