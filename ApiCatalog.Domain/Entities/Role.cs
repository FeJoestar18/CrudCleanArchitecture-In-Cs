namespace ApiCatalog.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public int? ParentRoleId { get; set; }
    public Role? ParentRole { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}

