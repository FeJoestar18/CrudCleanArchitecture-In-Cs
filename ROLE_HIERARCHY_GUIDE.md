# Sistema de Hierarquia de Roles

## Visão Geral

Sistema implementado com hierarquia dinâmica de roles baseada em níveis. Quanto maior o nível, maior o poder/permissão.

## Estrutura

### Tabela Roles
- **Id**: Identificador único
- **Name**: Nome do role (ex: "Usuario", "Funcionario", "Admin")
- **Level**: Nível hierárquico (1, 2, 3, etc.)
- **ParentRoleId**: Referência para role pai (opcional)

### Roles Padrão (criados automaticamente)
1. **Usuario** - Level 1 (base)
2. **Funcionario** - Level 2 (herda de Usuario)
3. **Admin** - Level 3 (herda de Funcionario)

## Como Funciona

### 1. Registro de Usuário
```json
POST /api/auth/register
{
  "username": "joao",
  "password": "senha123",
  "cpf": 12345678900,
  "email": "joao@email.com",
  "role": "Usuario"  // opcional, padrão é "Usuario"
}
```

### 2. Login
```json
POST /api/auth/login
{
  "email": "joao@email.com",
  "password": "senha123"
}
```

Retorna um JWT com claims:
- `ClaimTypes.Name`: email do usuário
- `ClaimTypes.Role`: nome do role
- `role_level`: nível hierárquico (usado para autorização)

### 3. Políticas de Autorização

#### Políticas Disponíveis:
- **UsuarioOrAbove**: Requer level >= 1 (todos autenticados)
- **FuncionarioOrAbove**: Requer level >= 2
- **AdminOnly**: Requer level >= 3

#### Uso em Controllers:
```csharp
[Authorize(Policy = "FuncionarioOrAbove")]
public IActionResult Create() { ... }
```

### 4. Validação Programática (em Services)

```csharp
public class MyService
{
    private readonly IAuthorizationService _authService;
    
    public async Task<bool> DoSomething(ClaimsPrincipal user)
    {
        // Verificar se usuário tem nível mínimo
        var authResult = await _authService.AuthorizeAsync(
            user, 
            null, 
            new MinimumRoleLevelRequirement(2)
        );
        
        if (!authResult.Succeeded)
            return false;
            
        // lógica...
        return true;
    }
}
```

## Adicionar Novos Níveis

### Opção 1: Via API (requer Admin)
```json
POST /api/role
{
  "name": "Gerente",
  "level": 4,
  "parentRoleId": 3  // herda de Admin
}
```

### Opção 2: Via Seed no AppDbContext
```csharp
modelBuilder.Entity<Role>().HasData(
    new Role { Id = 4, Name = "Gerente", Level = 4, ParentRoleId = 3 }
);
```

Depois criar migration:
```bash
dotnet ef migrations add AddGerenteRole --startup-project ../ApiCatalog.Api
dotnet ef database update --startup-project ../ApiCatalog.Api
```

### Opção 3: Criar Policy Customizada
```csharp
// Em AuthorizationExtensions.cs
options.AddPolicy("GerenteOrAbove", policy =>
    policy.Requirements.Add(new MinimumRoleLevelRequirement(4)));
```

## Exemplos de Uso

### Controller com Políticas
```csharp
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "UsuarioOrAbove")]
    public IActionResult GetAll() => Ok("Todos podem ver");

    [HttpPost]
    [Authorize(Policy = "FuncionarioOrAbove")]
    public IActionResult Create() => Ok("Só funcionários+");

    [HttpDelete]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Delete() => Ok("Só admins");
}
```

## Vantagens do Sistema

1. **Extensível**: Adicione novos níveis sem alterar código
2. **Hierárquico**: Níveis superiores herdam permissões dos inferiores
3. **Flexível**: Use políticas ou validação programática
4. **Centralizado**: Toda lógica em handlers reutilizáveis
5. **Type-safe**: Usa enumeração de níveis

## Arquivos Principais

- `Domain/Entities/Role.cs` - Entidade Role
- `Application/Policies/MinimumRoleLevelRequirement.cs` - Requisito de autorização
- `Application/Policies/MinimumRoleLevelHandler.cs` - Handler que valida requisito
- `Api/Extensions/AuthorizationExtensions.cs` - Configuração de políticas
- `Application/Services/AuthService.cs` - Emissão de tokens com claims
- `Application/Services/RoleService.cs` - Gerenciamento de roles
- `Infra/Context/AppDbContext.cs` - Seed de roles padrão

