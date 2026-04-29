using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateAccountDto : IValidatableObject
{
    [StringLength(50, MinimumLength = 2)]
    public string? City { get; set; }

    [StringLength(50, MinimumLength = 2)]
    public string? Country { get; set; }

    [StringLength(100, MinimumLength = 5)]
    public string? Address { get; set; }

    public string? CurrentPassword { get; set; }

    [StringLength(8, MinimumLength = 4)]
    public string? NewPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var wantsAddressUpdate = City is not null || Country is not null || Address is not null;
        if (wantsAddressUpdate &&
            (string.IsNullOrWhiteSpace(City) || string.IsNullOrWhiteSpace(Country) || string.IsNullOrWhiteSpace(Address)))
        {
            yield return new ValidationResult(
                "City, country and address are all required when updating address details.",
                [nameof(City), nameof(Country), nameof(Address)]);
        }

        var wantsPasswordUpdate = CurrentPassword is not null || NewPassword is not null;
        if (wantsPasswordUpdate &&
            (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword)))
        {
            yield return new ValidationResult(
                "CurrentPassword and NewPassword are both required when changing the password.",
                [nameof(CurrentPassword), nameof(NewPassword)]);
        }

        if (!string.IsNullOrWhiteSpace(CurrentPassword) &&
            !string.IsNullOrWhiteSpace(NewPassword) &&
            CurrentPassword == NewPassword)
        {
            yield return new ValidationResult(
                "NewPassword must be different from CurrentPassword.",
                [nameof(NewPassword)]);
        }

        if (!wantsAddressUpdate && !wantsPasswordUpdate)
        {
            yield return new ValidationResult(
                "At least one account detail or password change must be provided.",
                [nameof(City), nameof(Country), nameof(Address), nameof(CurrentPassword), nameof(NewPassword)]);
        }
    }
}
