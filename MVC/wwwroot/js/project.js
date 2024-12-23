const steps = document.querySelectorAll(".step");
const formSections = document.querySelectorAll(".form-section");
const nextButtons = document.querySelectorAll(".next-btn");
const backButtons = document.querySelectorAll(".back-btn");
const stepLines = document.querySelectorAll(".step-line");
const pincodeInput = document.getElementById("Pincode");
const showMoreButton = document.getElementById("showMoreFields");
const hiddenFields = document.getElementById("hiddenFields");
const pincodeInput1 = $("#Pincode");

let selectedAmenities;
let selectedValue;
let currentStep = 0;
let projectNameError = false;
const formData = {};

pincodeInput1.on("input", function () {
  let inputValue = $(this).val().trim();
  if (inputValue.length > 6) {
    inputValue = inputValue.slice(0, 6);
    $(this).val(inputValue);
  }
  if (inputValue.length === 6) {
    $(this).next(".error-message").remove();
  } else {
    if ($(this).next(".error-message").length === 0) {
      const errorMsg = `<span class="error-message">Pincode must be 6 digits.</span>`;
      $(this).after(errorMsg);
    }
  }
});

pincodeInput.addEventListener("input", function () {
  if (pincodeInput.value.trim() !== "") {
    showMoreButton.style.display = "inline-block";
  } else {
    showMoreButton.style.display = "none";
  }
});
showMoreButton.addEventListener("click", function () {
  const Pincode = document.querySelector("#Pincode");
  const errorMessage = Pincode.nextElementSibling;
  if (errorMessage && errorMessage.classList.contains("error-message")) {
    errorMessage.remove();
  }

  pincodeInput.disabled = false;
  showMoreButton.disabled = false;
});

$("#locality").on("change", function () {
  const inputValue = $(this).val();

  if (inputValue) {
    $(this).next(".error-message").remove();
  } else {
    if ($(this).next(".error-message").length === 0) {
      const errorMsg = `<span class="error-message">Please select a locality.</span>`;
      $(this).after(errorMsg);
    }
  }
});

$("#Address").on("input", function () {
  const inputValue = $(this).val().trim();

  if (inputValue) {
    $(this).next(".error-message").remove();
  } else {
    if ($(this).next(".error-message").length === 0) {
      const errorMsg = `<span class="error-message">Address is required.</span>`;
      $(this).after(errorMsg);
    }
  }
});

const rangeInput = document.getElementById("customRange1");
const valueBox = document.getElementById("rangeValue");

valueBox.textContent = rangeInput.value;

rangeInput.addEventListener("input", function () {
  valueBox.textContent = rangeInput.value;
  formData["ageOption"] = rangeInput.value;
});

function validateStep(step) {
  let isValid = true;

  $(formSections[step]).find(".error-message").remove();

  $(formSections[step])
    .find('input:radio, input:text, textarea, input[type="number"]')
    .each(function () {
      const $this = $(this);
      if ($this.is(":radio")) {
        if (
          $(`input[name="${$this.attr("name")}"]:checked`).length === 0 &&
          !$this.closest(".radio-group").next(".error-message").length
        ) {
          isValid = false;
          const errorMsg = `<span class="error-message">Please select an property option.</span>`;
          $this.closest(".radio-group").after(errorMsg);
        }
      } else if ($this.val().trim() === "") {
        isValid = false;
        const errorMsg = `<span class="error-message">This field is required.</span>`;
        $this.after(errorMsg);
      } else if ($this.attr("type") === "number" && $this.val() === "") {
        isValid = false;
        const errorMsg = `<span class="error-message">Please enter a number.</span>`;
        $this.after(errorMsg);
      }
    });

  //     const projectvalue = $("#property-Name");

  // if(!projectvalue.val()){
  //     const errorMsg = `<span class="error-message">Please enter project name.</span>`;
  //     $("#property-Name").after(errorMsg);
  // }
  //     const Addressval = $("#Addresse");
  //     console.log(Addressval);
  //     console.log(Addressval.val());

  // if(!projectvalue.val()){
  //     const errorMsg = `<span class="error-message">Please enter project name.</span>`;
  //     $("#property-Name").after(errorMsg);
  // }

  if (projectNameError) {
    isValid = false;
    const errorMsg = `<span class="error-message">Please enter a valid project name.</span>`;
    $("#property-Name").after(errorMsg);
  }

  const uploadStep = $(formSections[step]).find(".upload-container");
  const imageupload = $(formSections[step]).find(".imageupload");
  const videoupload = $(formSections[step]).find(".videoupload");
  const pdfadd = $(formSections[step]).find(".pdfadd");

  if (uploadStep.length > 0) {
    const uploadedImages = uploadStep
      .find(".uploaded-image")
      .filter(function () {
        return $(this).attr("src") && $(this).attr("src").trim() !== "";
      });

    if (uploadedImages.length === 0) {
      isValid = false;
      const errorMsg = `<span class="error-message">Please upload at least one image.</span>`;
      imageupload.after(errorMsg);
    }
  }

  const videoUploadStep = $(formSections[step]).find(".video-upload-container");
  if (videoUploadStep.length > 0) {
    const uploadedVideos = videoUploadStep
      .find("video source")
      .filter(function () {
        return $(this).attr("src") && $(this).attr("src").trim() !== "";
      });

    if (uploadedVideos.length === 0) {
      isValid = false;
      const errorMsg = `<span class="error-message">Please upload a video.</span>`;
      videoupload.after(errorMsg);
    }
  }

  const pdfStep = $(formSections[step]).find(".pdf-upload-area");
  if (pdfStep.length > 0) {
    const uploadedpdf = pdfStep.find(".pdf-preview").filter(function () {
      return $(this).attr("src") && $(this).attr("src").trim() !== "";
    });

    if (uploadedpdf.length === 0) {
      isValid = false;
      const errorMsg = `<span class="error-message">Please upload a brochure.</span>`;
      pdfadd.after(errorMsg);
    }
  }

  const Pincode = $(formSections[step]).find(".Pincode");
  if (Pincode.length > 0) {
    const pincodeValue = Pincode.val().trim();

    Pincode.next(".error-message").remove();

    if (pincodeValue.length !== 6 || isNaN(pincodeValue)) {
      isValid = false;
      const errorMsg = `<span class="error-message">Please enter a valid 6-digit pincode.</span>`;
      Pincode.after(errorMsg);
    }
  }
  const Locality = $(formSections[step]).find("#locality");
  if (Locality.length > 0) {
    const selectedValue = Locality.val();

    Locality.next(".error-message").remove();

    if (!selectedValue || selectedValue === "") {
      isValid = false;
      if (Locality.next(".error-message").length === 0) {
        const errorMsg = `<span class="error-message">Please select a Locality.</span>`;
        Locality.after(errorMsg);
      }

      if (Pincode.next(".error-message").length === 0) {
        const errormsg = `<span class="error-message">Please Click Get City.</span>`;
        Pincode.after(errormsg);
      }
    }
  }

  const statusOptions = $(formSections[step]).find(".stauts-option");
  if (
    statusOptions.length > 0 &&
    statusOptions.filter(".selected").length === 0
  ) {
    isValid = false;
    const errorMsg = `<span class="error-message">Please select an availability status.</span>`;
    statusOptions.closest(".furnishing-options").after(errorMsg);
  }

  const ageOptions = $(formSections[step]).find(".age-options");
  if (ageOptions.is(":visible")) {
  }

  return isValid;
}

function collectFormData() {
  $(formSections)
    .find("input, select, textarea")
    .each(function () {
      const $this = $(this);

      if ($this.is(":radio") || $this.is(":checkbox")) {
        formData[$this.attr("name")] = $this.is(":checked") ? true : false;
      } else {
        formData[$this.attr("name")] = $this.val().trim();
      }
    });

  const stautsOptions = $(formSections).find(".stauts-option");
  if (stautsOptions.length > 0) {
    const selectedstauts = stautsOptions.filter(".selected").data("value");
    if (selectedstauts) {
      formData["stautsOption"] = selectedstauts;
    }
  }

  return formData;
}

nextButtons.forEach((btn, index) => {
  btn.addEventListener("click", () => {
    if (validateStep(currentStep)) {
      formSections[currentStep].classList.remove("active");
      steps[currentStep].classList.remove("active");
      currentStep++;
      formSections[currentStep].classList.add("active");
      steps[currentStep].classList.add("active");
    }
  });
});

backButtons.forEach((btn, index) => {
  btn.addEventListener("click", () => {
    formSections[currentStep].classList.remove("active");
    steps[currentStep].classList.remove("active");
    currentStep--;
    formSections[currentStep].classList.add("active");
    steps[currentStep].classList.add("active");
  });
});

function selectOption(element) {
  const options = document.querySelectorAll(".option");
  options.forEach((option) => option.classList.remove("selected"));
  element.classList.add("selected");
}

const fileInput = document.getElementById("fileInput");
const uploadContainers = Array.from(
  document.querySelectorAll(".upload-container")
); // Convert NodeList to Array
let uploadedImages = [];
let fileNames = []; // To track uploaded image indices

function uploadimage() {
  fileInput.click();
}

fileInput.addEventListener("change", (event) => {
  const files = event.target.files;
  const fileInputimages = document.getElementById("fileInput");
  // Check if there is enough space for new images
  if (uploadedImages.length + files.length > uploadContainers.length) {
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "You can upload a maximum of 5 images.",
      confirmButtonText: "OK",
    });
    return;
  }

  Array.from(files).forEach((file) => {
    const validTypes = ["image/jpeg", "image/jpg", "image/png"];
    if (!validTypes.includes(file.type)) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "Invalid file type. Please upload an image type jpg/png/jpeg.",
        confirmButtonText: "OK",
      });
      return;
    }

    if (file.size > 15 * 1024 * 1024) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "File size exceeds 15mb. Please upload a smaller image.",
        confirmButtonText: "OK",
      });
      return;
    }

    const availableIndex = uploadContainers.findIndex(
      (container, index) => !uploadedImages.includes(index)
    );

    if (availableIndex !== -1) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const container = uploadContainers[availableIndex];
        const image = container.querySelector(".uploaded-image");
        image.src = e.target.result;
        container.style.display = "block";
        uploadedImages.push(availableIndex);
        fileNames.push(file);
      };

      reader.readAsDataURL(file);
    }
  });

  fileInput.value = "";
});

uploadContainers.forEach((container, index) => {
  const deleteButton = container.querySelector(".delete-icon");

  deleteButton.addEventListener("click", () => {
    const image = container.querySelector(".uploaded-image");
    image.src = "";
    container.style.display = "none";

    const imageIndex = uploadedImages.indexOf(index);
    if (imageIndex !== -1) {
      uploadedImages.splice(imageIndex, 1);
      fileNames.splice(imageIndex, 1);
    }
  });
});

const uploadvideobtn = document.querySelector(".upload-videos");
const videoFileInput = document.getElementById("videoFileInput");
const uploadVideoContainers = Array.from(
  document.querySelectorAll(".video-upload-container")
); // Convert to array for easy handling
let videoIndex = -1;
let videoFileName = "";

function uploadvideo(element) {
  videoFileInput.click();
}

videoFileInput.addEventListener("change", (event) => {
  const files = event.target.files;

  if (videoFileName?.name) {
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "You can upload a maximum of 1 video.",
      confirmButtonText: "OK",
    });
    return;
  }

  if (files.length > 0) {
    const file = files[0];

    const validTypes = ["video/mp4"];
    if (!validTypes.includes(file.type)) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "Invalid file type. Please upload an image or a video file.",
        confirmButtonText: "OK",
      });
      return;
    }

    if (file.size > 104857600) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "File size exceeds 15mb. Please upload a smaller Video.",
        confirmButtonText: "OK",
      });
      return;
    }

    const container = uploadVideoContainers[0]; // Always use the first container
    const video = container.querySelector(".video-preview");
    const source = video.querySelector("source");
    const videoURL = URL.createObjectURL(file);

    source.src = videoURL;
    video.load();
    container.style.display = "block";
    videoIndex = 0; // Mark video as uploaded
    videoFileName = file; // Save the file name

    videoFileInput.value = "";
  }
});
uploadVideoContainers.forEach((container, index) => {
  const deleteButton = container.querySelector(".delete-icon-video");

  deleteButton.addEventListener("click", () => {
    if (videoIndex === index) {
      const video = container.querySelector(".video-preview");
      const source = video.querySelector("source");

      // Clear the video source and reset the container
      source.src = "";
      video.load();
      videoFileName = "";

      container.style.display = "none";
      videoIndex = -1; // Reset video index
    }
  });
});

const pdfUploadButton = document.querySelector(".upload-pdfs");
const pdfFileInput = document.getElementById("pdfFileInput");
const pdfUploadContainer = document.querySelector(".pdf-upload-container"); // Single container
let pdfFileName = "";

function uploadpdf() {
  pdfFileInput.click();
}

pdfFileInput.addEventListener("change", (event) => {
  const files = event.target.files;

  if (files.size > 15 * 1024 * 1024) {
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "File size exceeds 15mb. Please upload a smaller image.",
      confirmButtonText: "OK",
    });
    return;
  }

  if (pdfFileName?.name) {
    Swal.fire({
      icon: "error",
      title: "Error",
      text: "You can upload only one brochure.",
      confirmButtonText: "OK",
    });
    return;
  }

  if (files.length === 1) {
    const file = files[0];
    const embedTag = pdfUploadContainer.querySelector(".pdf-preview");
    const deleteButton = pdfUploadContainer.querySelector(".delete-icon-pdf");
    if (file.size > 10 * 1024 * 1024) {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "File size exceeds 10mb. Please upload a smaller Brochure file.",
        confirmButtonText: "OK",
      });
      return;
    }

    embedTag.src = "";
    pdfUploadContainer.style.display = "none";

    const pdfURL = URL.createObjectURL(file);
    embedTag.src = pdfURL;
    pdfUploadContainer.style.display = "block";
    pdfFileName = file;

    pdfUploadContainer.dataset.pdfName = file.name;

    deleteButton.addEventListener("click", () => {
      embedTag.src = "";
      pdfUploadContainer.style.display = "none";
      pdfUploadContainer.dataset.pdfName = "";
      pdfFileInput.value = "";
      pdfFileName = "";
    });
  }

  pdfFileInput.value = "";
});
$("#pdfFileInput").on("change", function () {
  const pdfStep = $(".pdf-upload-area");
  const preview = pdfStep.find(".pdf-preview");
  const pdfadd = $(".pdfadd");

  const uploadedpdf = pdfStep.find(".pdf-preview").filter(function () {
    return $(this).attr("src") && $(this).attr("src").trim() !== "";
  });

  if (uploadedpdf.length > 0) {
    pdfadd.next(".error-message").remove();
  }
});
$("#videoFileInput").on("change", function () {
  const videouploadcontainer = $(".video-upload-container");
  const videoupload = $(".videoupload");
  const videoupload1 = videouploadcontainer
    .find("video source")
    .filter(function () {
      return $(this).attr("src") && $(this).attr("src").trim() !== "";
    });

  if (videoupload1.length > 0) {
    videoupload.next(".error-message").remove();
  }
});

$("#fileInput").on("change", function () {
  const imageupload = $(".imageupload");
  imageupload.next(".error-message").remove();
});

function removeError(container) {
  // Check if error message exists and remove it
  $(container).next(".error-message").remove();
}

function selectamemitiesOption(element) {
  const amemitiesOptions = document.querySelectorAll(".amemitiesOption");
  element.classList.toggle("selected");

  selectedAmenities = Array.from(
    document.querySelectorAll(".amemitiesOption.selected")
  ).map((option) => option.getAttribute("data-value"));
  formData["amenities"] = selectedAmenities;
}
function selectFurnishing(element) {
  const furnishingOptions = document.querySelectorAll(".furnishing-option");
  furnishingOptions.forEach((option) => option.classList.remove("selected"));
  element.classList.add("selected");
}
function selectstauts(element) {
  const stautsOptions = document.querySelectorAll(".stauts-option");
  stautsOptions.forEach((option) => option.classList.remove("selected"));
  element.classList.add("selected");
  removeError($(element).closest(".furnishing-options"));

  selectedValue = element.getAttribute("data-value");

  const ageOptionsDiv = document.querySelector(".age-options");

  if (element.textContent == "Ready to move") {
    ageOptionsDiv.style.display = "block";
  } else {
    ageOptionsDiv.style.display = "none";
  }
}

$("#property-Name").on("change", function () {
  const projectName = $(this).val();
  const projectvalue = document.getElementById("property-Name");

  if (projectName) {
    $.ajax({
      url: `http://localhost:5293/api/ProjectApi/ProjectName`,
      type: "POST", // HTTP method
      contentType: "application/json", // Content type
      data: JSON.stringify(projectName), // Send the project name as raw JSON
      success: function (response) {
        console.log("Project name is valid:", response);
        projectNameError = false;
        const projectvalue = $("#property-Name"); // Correct selection using jQuery

        if (projectvalue.length > 0) {
          // Ensure the element exists
          // Remove any existing error messages
          const existingErrorMsg = projectvalue.siblings(".error-message");
          if (existingErrorMsg.length) {
            existingErrorMsg.remove(); // Remove the error message
          }
        }
      },
      error: function (xhr, status, error) {
        console.error("Validation error:", xhr.responseText);

        const projectvalue = $("#property-Name"); // Correct selection using jQuery

        if (projectvalue.length > 0) {
          // Ensure the element exists
          // Remove any existing error messages
          const existingErrorMsg = projectvalue.siblings(".error-message");
          if (existingErrorMsg.length) {
            existingErrorMsg.remove(); // Remove the error message
          }
          projectNameError = true;

          // Add the error message below the input field
          const errormsg = `<span class="error-message">This Project Name is already registered.</span>`;
          projectvalue.after(errormsg);
        }
      },
    });
  } else {
    console.warn("Project name is empty.");
  }
});

const form = document.getElementById("myForm");

form.addEventListener("submit", (e) => {
  e.preventDefault(); // Prevent the default form submission

  // Collecting non-file inputs
  const formData = new FormData();
  formData.append(
    "ProjectName",
    document.getElementById("property-Name").value
  );
  formData.append("City", document.getElementById("city").value);
  formData.append("Locality", document.getElementById("locality").value);
  formData.append("Address", document.getElementById("Address").value);
  formData.append("Pincode", document.getElementById("Pincode").value);

  // Assuming you have a session user ID passed to ViewBag
  const userSessionId = loginuserid;
  formData.append("UserId", userSessionId);

  // Getting checkbox value for "Ready to Move"
  const selectedValue = document.querySelector(
    'input[name="ReadytoMove"]:checked'
  );
  formData.append("ReadytoMove", selectedValue ? true : false);

  // Collecting the file inputs
  const fileInputimages = document.getElementById("fileInput");
  const selectedFilesImage = fileInputimages.files;
  for (let i = 0; i < fileNames.length; i++) {
    formData.append("Files", fileNames[i]);
  }

  const videoFileInput = document.getElementById("videoFileInput");
  const selectedVideoFile = videoFileInput.files[0];

  if (videoFileName) {
    formData.append("VideoFiles", videoFileName);
  }

  const pdfFileInput = document.getElementById("pdfFileInput");
  const selectedPdfFile = pdfFileInput.files[0];
  if (pdfFileName) {
    formData.append("BorchureFile", pdfFileName);
  }

  if (Array.isArray(selectedAmenities) && selectedAmenities.length > 0) {
    // If selectedAmenities is a non-empty array, append each amenity
    selectedAmenities.forEach((amenity) => {
      formData.append("AllAmenities", amenity);
    });
  } else {
    // If it's null or an empty array, send null instead of an empty array
    formData.append("AllAmenities", 0);
  }

  for (let [key, value] of formData.entries()) {
    if (value instanceof File) {
      console.log(`${key}: ${value.name}`);
    } else {
      console.log(`${key}: ${value}`);
    }
  }

  $(document)
    .ajaxStart(function () {
      $("#loader").fadeIn();
    })
    .ajaxStop(function () {
      $("#loader").fadeOut();
    });

  $.ajax({
    url: "http://localhost:5293/api/ProjectApi/AddProject",
    type: "POST",
    data: formData,
    processData: false,
    contentType: false,
    success: function (response) {
      Swal.fire({
        icon: "success",
        title: "Success",
        text: response,
        confirmButtonText: "OK",
      });
      setTimeout(function () {
        window.location.href = "/client/user";
      }, 2000);
    },
    error: function (error) {
      Swal.fire({
        icon: "error",
        title: "error",
        text: error.responseJSON
          ? error.responseJSON
          : "An error occurred while adding the project.",
        confirmButtonText: "OK",
      });
    },
  });
});

document
  .getElementById("showMoreFields")
  .addEventListener("click", function () {
    const pincode = document.getElementById("Pincode").value.trim();
    const hiddenFields = document.getElementById("hiddenFields");
    const loader = document.getElementById("loader");

    // Check if pincode is valid (6 digits)
    if (pincode.length === 6) {
      loader.style.display = "flex";

      fetch(`https://api.postalpincode.in/pincode/${pincode}`)
        .then((response) => response.json())
        .then((data) => {
          if (data[0].Status === "Success") {
            const cityField = document.getElementById("city");
            const localitySelect = document.getElementById("locality");

            // Set city field and make it read-only (not editable)
            cityField.value = data[0].PostOffice[0].District;
            cityField.readOnly = true;

            // Populate locality dropdown with PostOffice names
            localitySelect.innerHTML =
              '<option value="" disabled selected>Select Locality</option>';
            data[0].PostOffice.forEach((office) => {
              const option = document.createElement("option");
              option.value = office.Name;
              option.textContent = office.Name;
              localitySelect.appendChild(option);
              hiddenFields.style.display = "block";
            });
          } else {
            Swal.fire({
              icon: "error",
              title: "error",
              text: "Invalid Pincode or no data found.",
              confirmButtonText: "OK",
            });
            hiddenFields.style.display = "none";
          }
        })
        .catch((error) => {
          Swal.fire({
            icon: "error",
            title: "error",
            text: "Unable to fetch data. Please try again later.",
            confirmButtonText: "OK",
          });
        })
        .finally(() => {
          loader.style.display = "none";
        });
    } else {
      alert("Please enter a valid 6-digit Pincode.");
    }
  });

// Show "Get City" button dynamically when valid pincode is entered
document.getElementById("Pincode").addEventListener("input", function () {
  const showMoreFields = document.getElementById("showMoreFields");
  if (this.value.trim().length === 6) {
    showMoreFields.style.display = "block";
  } else {
    showMoreFields.style.display = "none";
    document.getElementById("hiddenFields").style.display = "none";
  }
});
