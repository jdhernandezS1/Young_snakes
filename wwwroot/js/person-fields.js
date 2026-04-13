// wwwroot/js/person-fields.js
function initPersonFields(playerRoleId) {
    const roleSelect = document.getElementById("roleSelect");
    const playerFields = document.querySelectorAll(".player-fields");
    const staffFields = document.querySelectorAll(".staff-fields");

    function updateVisibility() {
        const selectedRole = roleSelect.value;

        if (selectedRole === "") {
            // Si no hay rol, ocultamos todo
            playerFields.forEach(el => el.style.display = "none");
            staffFields.forEach(el => el.style.display = "none");
        } else if (selectedRole === playerRoleId.toString()) {
            // Es Jugador: Ver altura/jersey
            playerFields.forEach(el => el.style.display = "block");
            staffFields.forEach(el => el.style.display = "none");
        } else {
            // Es Staff: Ver contacto
            playerFields.forEach(el => el.style.display = "none");
            staffFields.forEach(el => el.style.display = "block");
        }
    }

    if (roleSelect) {
        roleSelect.addEventListener("change", updateVisibility);
        updateVisibility(); // Ejecutar al cargar
    }
}