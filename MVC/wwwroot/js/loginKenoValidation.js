
// $(document).ready(function () {
//     var validator = $(".login").kendoValidator({
//         rules: {
//             required: function (input) {
//                 return $.trim(input.val()) !== "";
//             },
//             email: function (input) {
//                 if (input.is("#loginEmail") && input.val()) {
//                     // Email validation regex
//                     return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.val());
//                 }
//                 return true;
//             },
//             strongPassword: function (input) {
//                 if (input.is("#loginPassword") && input.val()) {
//                     // Strong password regex
//                     return /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$/.test(input.val());
//                 }
//                 return true;
//             },
//             checkboxChecked: function (input) {
//                 if (input.is("#acceptTs")) {
//                     return input.is(":checked");
//                 }
//                 return true;
//             }
//         },
//         messages: {
//             required: "This field is required.",
//             email: "Please enter a valid email address.",
//             strongPassword: "Password must be at least 8 characters, include uppercase, lowercase, digits, and a special character.",
//             checkboxChecked: "You must accept the end user License agreement.",
//         },
//         errorTemplate: "<span class='custom-error'>#=message#</span>",
//         // errorPlacement: function (error, inputElement) {
//         //                 // Place the error message directly below the input field
//         //                 console.log("yvt")
//         //                 error.insertAfter(inputElement);
//         //             }
//     }).data("kendoValidator");

    var validatorForgorPassword = $("#ForgotPasswordWindow").kendoValidator({
        rules: {
            required: function (input) {
                return $.trim(input.val()) !== "";
            },
            email: function (input) {
                if (input.is("#forgotPasswordEmail") && input.val()) {
                    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.val());
                }
                return true;
            },
        },
        messages: {
            required: "This field is required.",
            email: "Please enter a valid email address.",
        },
        errorTemplate: "<span class='custom-error'>#=message#</span>",
    }).data("kendoValidator");

//     $("#forgotpasswordbtn").on("click", function (e) {
//         e.preventDefault();
//         if (validatorForgorPassword.validate()) {
//             $("#ForgotPasswordWindow").data("kendoWindow").close();
//             // Swal.fire({
//             //     icon: 'success',
//             //     title: 'Success!',
//             //     text: 'Check your mail'
//             // });
//         } else {
//             console.log("error");
//         }
//     });


//     // Handle form submission
//     $(".loginbtn").on("click", function (e) {
//         e.preventDefault();
//         if (validator.validate()) {
//             alert("Form is valid and ready to submit!");
//         } else {
//             alert("Please correct the errors in the form.");
//         }
//     });
// });

// $(document).ready(function () {
//     var validator = $(".login").kendoValidator({
//         rules: {
//             required: function (input) {
//                 return $.trim(input.val()) !== "";
//             },
//             email: function (input) {
//                 if (input.is("#loginEmail") && input.val()) {
//                     // Email validation regex
//                     return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.val());
//                 }
//                 return true;
//             },
//             strongPassword: function (input) {
//                 if (input.is("#loginPassword") && input.val()) {
//                     // Strong password regex
//                     return /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$/.test(input.val());
//                 }
//                 return true;
//             },
//             checkboxChecked: function (input) {
//                 if (input.is("#acceptTs")) {
//                     return input.is(":checked");
//                 }
//                 return true;
//             }
//         },
//         messages: {
//             required: "This field is required.",
//             email: "Please enter a valid email address.",
//             strongPassword: "Password must be at least 8 characters, include uppercase, lowercase, digits, and a special character.",
//             checkboxChecked: "You must accept the end user License agreement.",
//         },
//         errorTemplate: "<span class='custom-error'>#=message#</span>",
//         // errorPlacement: function (error, inputElement) {
//         //                 // Place the error message directly below the input field
//         //                 console.log("yvt")
//         //                 error.insertAfter(inputElement);
//         //             }
//     }).data("kendoValidator");


//     var validatorForgorPassword = $("#ForgotPasswordWindow").kendoValidator({
//         rules: {
//             required: function (input) {
//                 return $.trim(input.val()) !== "";
//             },
//             email: function (input) {
//                 if (input.is("#forgotPasswordEmail") && input.val()) {
//                     return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(input.val());
//                 }
//                 return true;
//             },
//         },
//         messages: {
//             required: "This field is required.",
//             email: "Please enter a valid email address.",
//         },
//         errorTemplate: "<span class='custom-error'>#=message#</span>",
//     }).data("kendoValidator");

//     $(".loginbtn").on("click", function (e) {
//         e.preventDefault();
//         if (validator.validate()) {
//             // alert("Form is valid and ready to submit!");
//         } else {
//             //alert("Please correct the errors in the form.");
//         }
//     });


    $("#forgotpasswordbtn").on("click", function (e) {
        e.preventDefault();
        if (validatorForgorPassword.validate()) {
            $("#ForgotPasswordWindow").data("kendoWindow").close();
            // Swal.fire({
            //     icon: 'success',
            //     title: 'Success!',
            //     text: 'Check your mail'
            // });
        } else {
            console.log("error");
        }
    });

//     $(document).ready(function () {
//         var validator = $("#passwordForm").kendoValidator({
//             rules: {
//                 required: function (input) {
//                     if (input.is("#ConformPassword") || input.is("#NewPassword")) {
//                         return $.trim(input.val()) !== "";
//                     }
//                     return true;
//                 },
//                 matchPassword: function (input) {
//                     if (input.is("#ConformPassword")) {
//                         return input.val() === $("#NewPassword").val();
//                     }
//                     return true;
//                 },
//                 strongPassword: function (input) {
//                     if (input.is("#NewPassword") && input.val()) {
//                         return /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$/.test(input.val());
//                     }
//                     return true;
//                 },
//             },
//             messages: {
//                 required: "This field is required",
//                 matchPassword: "Passwords do not match",
//                 strongPassword: "Password must be at least 8 characters, include uppercase, lowercase, digits, and a special character.",
//             },
//             errorTemplate: "<span class='custom-error'>#=message#</span>",
//         }).data("kendoValidator");

//         $("#btnChangePassword").click(function () {
//             if (validator.validate()) {
//                 return true;
//             } else {
//                 return false;
//             }
//         });
//     });

// });

$("#forgotpasswordbtn").on("click", function (e) {
    e.preventDefault();
    if (validatorForgorPassword.validate()) {
        $("#ForgotPasswordWindow").data("kendoWindow").close();
        // Swal.fire({
        //     icon: 'success',
        //     title: 'Success!',
        //     text: 'Check your mail'
        // });
        
    } else {
        console.log("error");
    }
});

 $(document).ready(function () {
        var validator = $("#passwordForm").kendoValidator({
            rules: {
                required: function (input) {
                    if (input.is("#ConformPassword") || input.is("#NewPassword")) {
                        return $.trim(input.val()) !== "";
                    }
                    return true;
                },
                matchPassword: function (input) {
                    if (input.is("#ConformPassword")) {
                        return input.val() === $("#NewPassword").val();
                    }
                    return true;
                },
                strongPassword: function (input) {
                    if (input.is("#NewPassword") && input.val()) {
                        return /^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_])[A-Za-z\d\W_]{8,}$/.test(input.val());
                    }
                    return true;
                },
            },
            messages: {
                required: "This field is required",
                matchPassword: "Passwords do not match",
                strongPassword: "Password must be at least 8 characters, include uppercase, lowercase, digits, and a special character.",
            },
            errorTemplate: "<span class='custom-error'>#=message#</span>",
        }).data("kendoValidator");

        $("#btnChangePassword").click(function () {
            if (validator.validate()) {
                return true;
            } else {
                return false;
            }
        });
    });
