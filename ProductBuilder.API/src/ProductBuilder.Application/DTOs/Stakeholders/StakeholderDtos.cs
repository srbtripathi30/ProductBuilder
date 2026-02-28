namespace ProductBuilder.Application.DTOs.Stakeholders;

public class InsurerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInsurerRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdateInsurerRequest : CreateInsurerRequest
{
    public bool IsActive { get; set; }
}

public class UnderwriterDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public string? Specialization { get; set; }
    public decimal? AuthorityLimit { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUnderwriterRequest
{
    public Guid UserId { get; set; }
    public string? LicenseNo { get; set; }
    public string? Specialization { get; set; }
    public decimal? AuthorityLimit { get; set; }
}

public class UpdateUnderwriterRequest
{
    public string? LicenseNo { get; set; }
    public string? Specialization { get; set; }
    public decimal? AuthorityLimit { get; set; }
}

public class BrokerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid? InsurerId { get; set; }
    public string? InsurerName { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public decimal? CommissionRate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBrokerRequest
{
    public Guid UserId { get; set; }
    public Guid? InsurerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public decimal? CommissionRate { get; set; }
}

public class UpdateBrokerRequest
{
    public Guid? InsurerId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? LicenseNo { get; set; }
    public decimal? CommissionRate { get; set; }
    public bool IsActive { get; set; }
}
