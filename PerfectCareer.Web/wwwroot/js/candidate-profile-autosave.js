(() => {
    "use strict";

    const form =
        document.getElementById("pc-profile-edit-form");

    if (!form) {
        return;
    }

    const autoSaveUrl =
        form.dataset.autoSaveUrl;

    const rowVersionInput =
        document.getElementById("pc-profile-row-version");

    const antiForgeryToken =
        form.querySelector(
            'input[name="__RequestVerificationToken"]');

    const firstNameInput =
        document.getElementById("FirstName");

    const lastNameInput =
        document.getElementById("LastName");

    const locationInput =
        document.getElementById("Location");

    const status =
        document.getElementById("pc-profile-save-status");

    const reloadButton =
        document.getElementById("pc-profile-reload");

    const submitButton =
        document.getElementById("pc-profile-submit");

    if (!autoSaveUrl ||
        !rowVersionInput ||
        !antiForgeryToken ||
        !firstNameInput ||
        !lastNameInput ||
        !locationInput ||
        !status ||
        !reloadButton ||
        !submitButton) {
        return;
    }

    const trackedInputs = [
        firstNameInput,
        lastNameInput,
        locationInput
    ];

    let changeNumber = 0;
    let hasUnsavedChanges = false;
    let isSaving = false;
    let hasConflict = false;
    let isSubmitting = false;

    function showStatus(message, colorClass) {
        status.textContent = message;

        status.classList.remove(
            "text-muted",
            "text-warning",
            "text-success",
            "text-danger");

        status.classList.add(colorClass);
    }

    function validateFields() {
        const firstName = firstNameInput.value.trim();
        const lastName = lastNameInput.value.trim();
        const location = locationInput.value.trim();

        if (!firstName || !lastName || !location) {
            return "Complete the required fields before auto-save.";
        }

        if (firstName.length > 100 ||
            lastName.length > 100) {
            return "First name and last name must not exceed 100 characters.";
        }

        if (location.length > 150) {
            return "Location must not exceed 150 characters.";
        }

        return null;
    }

    async function saveChanges() {
        if (!hasUnsavedChanges ||
            isSaving ||
            hasConflict ||
            isSubmitting) {
            return;
        }

        const validationMessage =
            validateFields();

        if (validationMessage) {
            showStatus(
                validationMessage,
                "text-warning");

            return;
        }

        const savedChangeNumber =
            changeNumber;

        isSaving = true;
        submitButton.disabled = true;

        showStatus(
            "Saving...",
            "text-muted");

        try {
            const response = await fetch(
                autoSaveUrl,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type": "application/json",
                        "RequestVerificationToken":
                            antiForgeryToken.value
                    },
                    body: JSON.stringify({
                        firstName: firstNameInput.value,
                        lastName: lastNameInput.value,
                        location: locationInput.value,
                        rowVersion: rowVersionInput.value
                    })
                });

            const result = await response
                .json()
                .catch(() => ({}));

            if (response.status === 409) {
                hasConflict = true;
                reloadButton.classList.remove("d-none");

                showStatus(
                    result.message ??
                    "This profile changed in another session.",
                    "text-danger");

                return;
            }

            if (!response.ok) {
                showStatus(
                    result.message ??
                    "Auto-save failed. It will retry.",
                    "text-danger");

                return;
            }

            rowVersionInput.value =
                result.rowVersion;

            if (changeNumber === savedChangeNumber) {
                hasUnsavedChanges = false;

                showStatus(
                    "All changes saved",
                    "text-success");
            }
            else {
                showStatus(
                    "Unsaved changes",
                    "text-warning");
            }
        }
        catch {
            showStatus(
                "Connection problem. Auto-save will retry.",
                "text-danger");
        }
        finally {
            isSaving = false;

            if (!isSubmitting) {
                submitButton.disabled = false;
            }
        }
    }

    trackedInputs.forEach(input => {
        input.addEventListener("input", () => {
            changeNumber += 1;
            hasUnsavedChanges = true;

            if (!hasConflict) {
                showStatus(
                    "Unsaved changes",
                    "text-warning");
            }
        });
    });

    reloadButton.addEventListener("click", () => {
        window.location.reload();
    });

    form.addEventListener("submit", event => {
        if (isSaving) {
            event.preventDefault();

            showStatus(
                "Wait for the current auto-save to finish.",
                "text-warning");

            return;
        }

        isSubmitting = true;
    });

    window.addEventListener("beforeunload", event => {
        if (!hasUnsavedChanges || isSubmitting) {
            return;
        }

        event.preventDefault();
        event.returnValue = "";
    });

    window.setInterval(
        saveChanges,
        5000);
})();