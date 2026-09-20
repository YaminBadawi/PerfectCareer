(() => {
    "use strict";

    const configuration =
        window.perfectCareerCv;

    if (!configuration) {
        return;
    }

    const securityToken =
        document.querySelector(
            '#pc-cv-security-form ' +
            'input[name="__RequestVerificationToken"]');

    const status =
        document.getElementById(
            "pc-cv-save-status");

    const errorBox =
        document.getElementById(
            "pc-cv-save-error");

    const publishButton =
        document.getElementById(
            "pc-cv-publish");

    const publishHelp =
        document.getElementById(
            "pc-cv-publish-help");

    const profileRowVersion =
        document.getElementById(
            "pc-cv-profile-row-version");

    const firstNameInput =
        document.getElementById(
            "pc-cv-first-name");

    const lastNameInput =
        document.getElementById(
            "pc-cv-last-name");

    const locationInput =
        document.getElementById(
            "pc-cv-location");

    const attributeInputs =
        Array.from(
            document.querySelectorAll(
                ".pc-cv-attribute-value"));

    if (!securityToken ||
        !status ||
        !profileRowVersion ||
        !firstNameInput ||
        !lastNameInput ||
        !locationInput) {
        return;
    }

    let sequence = 0;
    let profileDirtyVersion = null;
    let isSaving = false;
    let hasConflict = false;

    const dirtyAttributes =
        new Map();

    const initiallyMissingBooleans =
        new Set();

    document.querySelectorAll(
        "[data-cv-attribute]"
    ).forEach(article => {
        const firstControl =
            article.querySelector(
                ".pc-cv-attribute-value");

        if (!firstControl) {
            return;
        }

        if (Number(
                firstControl.dataset.dataType) === 7 &&
            article.classList.contains("missing")) {
            initiallyMissingBooleans.add(
                firstControl.dataset.definitionId);
        }
    });

    const touchedBooleans =
        new Set();

    function setStatus(
        message,
        colorClass) {
        status.textContent = message;

        status.classList.remove(
            "text-muted",
            "text-warning",
            "text-success",
            "text-danger");

        status.classList.add(
            colorClass);
    }

    function showError(message) {
        errorBox.textContent = message;
        errorBox.hidden = false;
    }

    function hideError() {
        errorBox.textContent = "";
        errorBox.hidden = true;
    }

    function hasPendingChanges() {
        return profileDirtyVersion !== null ||
            dirtyAttributes.size > 0;
    }

    function controlsFor(definitionId) {
        return attributeInputs.filter(
            input =>
                input.dataset.definitionId ===
                definitionId);
    }

    function createAttributePayload(
        definitionId) {
        const controls =
            controlsFor(definitionId);

        const firstControl =
            controls[0];

        const dataType =
            Number(
                firstControl.dataset.dataType);

        const payload = {
            attributeDefinitionId:
                Number(definitionId),

            rowVersion:
                firstControl.dataset.rowVersion
        };

        switch (dataType) {
            case 1:
            case 2:
            case 3:
                payload.textValue =
                    firstControl.value || null;
                break;

            case 4:
                payload.numberValue =
                    firstControl.value === ""
                        ? null
                        : Number(
                            firstControl.value);
                break;

            case 5:
                payload.dateValue =
                    firstControl.value || null;
                break;

            case 6: {
                const start =
                    controls.find(
                        input =>
                            input.dataset.periodPart ===
                            "start");

                const end =
                    controls.find(
                        input =>
                            input.dataset.periodPart ===
                            "end");

                payload.periodStart =
                    start?.value || null;

                payload.periodEnd =
                    end?.value || null;

                break;
            }

            case 7:
                payload.booleanValue =
                    firstControl.checked;
                break;

            case 8:
                payload.selectedOptionId =
                    firstControl.value === ""
                        ? null
                        : Number(
                            firstControl.value);
                break;
        }

        return payload;
    }

    function isAttributeMissing(
        definitionId) {
        const controls =
            controlsFor(definitionId);

        if (controls.length === 0) {
            return true;
        }

        const firstControl =
            controls[0];

        const dataType =
            Number(
                firstControl.dataset.dataType);

        switch (dataType) {
            case 1:
            case 2:
            case 3:
                return firstControl
                    .value
                    .trim() === "";

            case 4:
            case 5:
            case 8:
                return firstControl.value === "";

            case 6: {
                const start =
                    controls.find(
                        input =>
                            input.dataset.periodPart ===
                            "start");

                const end =
                    controls.find(
                        input =>
                            input.dataset.periodPart ===
                            "end");

                return !start?.value ||
                    !end?.value;
            }

            case 7:
                return initiallyMissingBooleans
                    .has(definitionId) &&
                    !touchedBooleans
                        .has(definitionId);

            default:
                return true;
        }
    }

    function updateAttributeState(
        definitionId) {
        const article =
            document.querySelector(
                `[data-cv-attribute="${definitionId}"]`);

        if (!article) {
            return;
        }

        const missing =
            isAttributeMissing(
                definitionId);

        article.classList.toggle(
            "missing",
            missing);

        const controlContainer =
            article.querySelector(
                ".pc-cv-attribute-control");

        let missingMessage =
            article.querySelector(
                ".pc-cv-missing-message");

        if (missing &&
            !missingMessage &&
            controlContainer) {
            missingMessage =
                document.createElement("p");

            missingMessage.className =
                "pc-cv-missing-message";

            missingMessage.textContent =
                "This information is required.";

            controlContainer.appendChild(
                missingMessage);
        }

        if (missingMessage) {
            missingMessage.hidden =
                !missing;
        }
    }

    function updateProfileState() {
        [
            firstNameInput,
            lastNameInput,
            locationInput
        ].forEach(input => {
            input.classList.toggle(
                "is-missing",
                input.value.trim() === "");
        });
    }

    function updateCompletion() {
        updateProfileState();

        const profileComplete =
            firstNameInput.value.trim() !== "" &&
            lastNameInput.value.trim() !== "" &&
            locationInput.value.trim() !== "" &&
            Boolean(
                document.querySelector(
                    ".pc-cv-photo img"));

        const attributeArticles =
            Array.from(
                document.querySelectorAll(
                    "[data-cv-attribute]"));

        const attributesComplete =
            attributeArticles.every(
                article =>
                    !article.classList
                        .contains("missing"));

        const complete =
            profileComplete &&
            attributesComplete;

        if (publishButton) {
            publishButton.disabled =
                !complete;
        }

        if (publishHelp) {
            publishHelp.hidden =
                complete;
        }
    }

    function validateProfile() {
        const firstName =
            firstNameInput.value.trim();

        const lastName =
            lastNameInput.value.trim();

        const location =
            locationInput.value.trim();

        if (!firstName ||
            !lastName ||
            !location) {
            return "First name, last name, and location are required.";
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

    async function readResponse(response) {
        return response
            .json()
            .catch(() => ({}));
    }

    async function saveProfile(
        savedVersion) {
        const validationMessage =
            validateProfile();

        if (validationMessage) {
            throw new Error(
                validationMessage);
        }

        const response =
            await fetch(
                configuration.profileSaveUrl,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            securityToken.value
                    },
                    body: JSON.stringify({
                        firstName:
                            firstNameInput.value,

                        lastName:
                            lastNameInput.value,

                        location:
                            locationInput.value,

                        rowVersion:
                            profileRowVersion.value
                    })
                });

        const result =
            await readResponse(response);

        if (!response.ok) {
            const error =
                new Error(
                    result.message ||
                    "Profile information could not be saved.");

            error.isConflict =
                response.status === 409;

            throw error;
        }

        profileRowVersion.value =
            result.rowVersion;

        if (profileDirtyVersion ===
            savedVersion) {
            profileDirtyVersion = null;
        }
    }

    async function saveAttribute(
        definitionId,
        savedVersion) {
        const response =
            await fetch(
                configuration.attributeSaveUrl,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            securityToken.value
                    },
                    body: JSON.stringify(
                        createAttributePayload(
                            definitionId))
                });

        const result =
            await readResponse(response);

        if (!response.ok) {
            const error =
                new Error(
                    result.message ||
                    "The profile value could not be saved.");

            error.isConflict =
                response.status === 409;

            throw error;
        }

        controlsFor(
            definitionId
        ).forEach(input => {
            input.dataset.rowVersion =
                result.rowVersion;
        });

        if (dirtyAttributes.get(
                definitionId) ===
            savedVersion) {
            dirtyAttributes.delete(
                definitionId);
        }
    }

    async function saveChanges() {
        if (isSaving ||
            hasConflict ||
            !hasPendingChanges()) {
            return;
        }

        isSaving = true;
        hideError();

        setStatus(
            "Saving...",
            "text-muted");

        const savedProfileVersion =
            profileDirtyVersion;

        const savedAttributes =
            Array.from(
                dirtyAttributes.entries());

        const errors = [];

        if (savedProfileVersion !== null) {
            try {
                await saveProfile(
                    savedProfileVersion);
            }
            catch (error) {
                errors.push(
                    error.message);

                if (error.isConflict) {
                    hasConflict = true;
                }
            }
        }

        for (const [
            definitionId,
            savedVersion
        ] of savedAttributes) {
            if (hasConflict) {
                break;
            }

            try {
                await saveAttribute(
                    definitionId,
                    savedVersion);
            }
            catch (error) {
                errors.push(
                    error.message);

                if (error.isConflict) {
                    hasConflict = true;
                }
            }
        }

        isSaving = false;

        if (errors.length > 0) {
            showError(errors[0]);

            setStatus(
                hasConflict
                    ? "Conflict detected — reload the page"
                    : "Save failed — it will retry",
                "text-danger");

            return;
        }

        if (hasPendingChanges()) {
            setStatus(
                "Unsaved changes",
                "text-warning");
        }
        else {
            setStatus(
                "All changes saved",
                "text-success");
        }

        updateCompletion();
    }

    [
        firstNameInput,
        lastNameInput,
        locationInput
    ].forEach(input => {
        input.addEventListener(
            "input",
            () => {
                sequence += 1;
                profileDirtyVersion =
                    sequence;

                hideError();
                updateCompletion();

                setStatus(
                    "Unsaved changes",
                    "text-warning");
            });
    });

    attributeInputs.forEach(input => {
        input.addEventListener(
            "input",
            () => {
                const definitionId =
                    input.dataset.definitionId;

                sequence += 1;

                dirtyAttributes.set(
                    definitionId,
                    sequence);

                if (Number(
                        input.dataset.dataType) === 7) {
                    touchedBooleans.add(
                        definitionId);
                }

                hideError();

                updateAttributeState(
                    definitionId);

                updateCompletion();

                setStatus(
                    "Unsaved changes",
                    "text-warning");
            });
    });

    document.querySelectorAll(
        ".pc-cv-project-choice"
    ).forEach(choice => {
        choice.addEventListener(
            "change",
            () => {
                choice
                    .closest(
                        ".pc-cv-project")
                    ?.classList
                    .toggle(
                        "selected",
                        choice.checked);
            });
    });

    window.addEventListener(
        "beforeunload",
        event => {
            if (!hasPendingChanges()) {
                return;
            }

            event.preventDefault();
            event.returnValue = "";
        });

    document.querySelectorAll(
        "[data-cv-attribute]"
    ).forEach(article => {
        updateAttributeState(
            article.dataset.cvAttribute);
    });

    updateCompletion();

    window.setInterval(
        saveChanges,
        5000);
})();