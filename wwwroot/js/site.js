// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function() {
    // Get mobile menu elements
    const mobileMenuButton = document.getElementById('mobile-menu-button');
    const mobileMenu = document.getElementById('mobile-menu');
    
    // Toggle mobile menu
    if (mobileMenuButton && mobileMenu) {
        mobileMenuButton.addEventListener('click', function() {
            if (mobileMenu.classList.contains('max-h-0')) {
                mobileMenu.classList.remove('max-h-0');
                mobileMenu.classList.add('max-h-60'); // Adjust height as needed
            } else {
                mobileMenu.classList.remove('max-h-60');
                mobileMenu.classList.add('max-h-0');
            }
        });
    }
    
    // Registration popup functionality
    const registerButtons = document.querySelectorAll('.register-company-btn');
    const registerPopup = document.getElementById('register-popup');
    const closePopupButton = document.getElementById('close-popup');
    
    if (registerButtons.length > 0 && registerPopup && closePopupButton) {
        // Open popup when register buttons are clicked
        registerButtons.forEach(button => {
            button.addEventListener('click', function() {
                registerPopup.classList.add('active');
                document.body.style.overflow = 'hidden'; // Prevent scrolling
            });
        });
        
        // Close popup when close button is clicked
        closePopupButton.addEventListener('click', function() {
            registerPopup.classList.remove('active');
            document.body.style.overflow = ''; // Re-enable scrolling
        });
        
        // Close popup when clicking outside the content
        registerPopup.addEventListener('click', function(e) {
            if (e.target === registerPopup) {
                registerPopup.classList.remove('active');
                document.body.style.overflow = ''; // Re-enable scrolling
            }
        });
    }
    
    // Handle registration button clicks
    document.addEventListener('DOMContentLoaded', function() {
        const registerButtons = document.querySelectorAll('.register-company-btn');
        
        registerButtons.forEach(button => {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                window.location.href = '/Home/RegisterCompany';
            });
        });
    });
});
