namespace ApiCatalog.Application.Common;

public static class Messages
{
    public static class Auth
    {
        public const string UserAlreadyExists = "Usuário já existe";
        public const string UserRegistered = "Usuário registrado com sucesso";
        public const string InvalidCredentials = "Usuário ou senha inválidos";
        public const string UserNotFound = "Usuário não encontrado";
        public const string Unauthenticated = "Usuário não autenticado. Faça login e tente novamente.";
        public const string MissingClaims = "Sessão inválida: claims de identidade ausentes.";
        public const string LoginSuccessful = "Login realizado com sucesso";

        public const string UsernameRequired = "Username obrigatório.";
        public const string PasswordRequired = "Senha obrigatória.";
        public const string CpfRequired = "CPF obrigatório.";
        public const string EmailAlreadyExists = "Email já cadastrado.";
        public const string CpfAlreadyExists = "CPF já cadastrado.";
        public const string WeakPassword = "Senha fraca. Use ao menos 6 caracteres, com letras e números.";
        public const string InvalidCpf = "CPF inválido.";
    }
    
    public static class Logout 
    {
        public const string LogoutSuccessful = "Logout realizado com sucesso";
    }
    
    public static class Products
    {
        public const string ProductNotFound = "Produto não encontrado";
        
        public const string ProductCreated = "Produto criado com sucesso";
        public const string ProductUpdated = "Produto atualizado com sucesso";
        public const string ProductDeleted = "Produto deletado com sucesso";
        
        public const string DeletionRequested = "Solicitação de deleção enviada para aprovação do admin";
        public const string DeletionApproved = "Deleção aprovada e produto removido";
        public const string DeletionRejected = "Solicitação de deleção rejeitada";
        public const string NotPendingDeletion = "Este produto não está pendente de deleção";
        public const string CannotEditPendingDeletion = "Produto pendente de deleção não pode ser editado";
        
        public const string ProductPurchased = "Produto adquirido com sucesso";
        public const string ProductUnavailable = "Produto não disponível para compra";
        public const string InsufficientStock = "Estoque insuficiente. Disponível: {0}";
        public const string PendingDeletionProducts = "Produtos pendentes de deleção";
        
        public const string OnlyEmployeesCanCreate = "Apenas funcionários ou admins podem criar produtos";
        public const string OnlyEmployeesCanEdit = "Apenas funcionários ou admins podem editar produtos";
        public const string OnlyEmployeesCanDelete = "Você não tem permissão para deletar produtos";
        public const string OnlyAdminsCanViewPending = "Apenas admins podem visualizar solicitações de deleção";
        public const string OnlyAdminsCanApprove = "Apenas admins podem aprovar deleções";
        public const string OnlyAdminsCanReject = "Apenas admins podem rejeitar deleções";
    }

    public static class JsonResponsesApi
    {
        public const string Success = "Operação realizada com sucesso";
        public const string Failure = "Falha ao realizar a operação";
        public const string InvalidJson = "Corpo JSON inválido ou Content-Type ausente.";
        
        public static string? InvalidRequest;
    }
    
    public static class Roles
    {
        public const string RoleNotFound = "Role não encontrada";
        public const string RoleCreated = "Role criada com sucesso";
        public const string InsufficientPermissions = "Permissões insuficientes para realizar esta ação";
    }
}