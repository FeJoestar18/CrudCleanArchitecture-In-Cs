# Sistema de Catálogo de Produtos com Permissões Hierárquicas

## 📦 Visão Geral

Sistema completo de catálogo de produtos implementado com Clean Architecture e permissões baseadas em hierarquia de roles. O sistema permite diferentes níveis de acesso e controle conforme o papel do usuário.

## 🎭 Permissões por Role

### 👤 Usuario (Level 1)
- ✅ **Visualizar** produtos disponíveis
- ✅ **Comprar** produtos (adquirir)
- ✅ **Ver** suas próprias compras
- ❌ **NÃO pode** criar, editar ou deletar produtos

### 👷 Funcionario (Level 2)
- ✅ **Tudo que Usuario pode fazer**
- ✅ **Criar** novos produtos
- ✅ **Editar** produtos existentes
- ✅ **Solicitar deleção** de produtos (requer aprovação do Admin)
- ✅ **Comprar** produtos
- ❌ **NÃO pode** deletar produtos diretamente
- ❌ **NÃO pode** aprovar/rejeitar deleções

### 👑 Admin (Level 3)
- ✅ **CRUD completo** sem restrições
- ✅ **Criar** produtos
- ✅ **Editar** produtos
- ✅ **Deletar** produtos **imediatamente** (sem aprovação)
- ✅ **Aprovar/Rejeitar** solicitações de deleção de funcionários
- ✅ **Ver produtos inativos** e pendentes
- ✅ **Comprar** produtos

## 🏗️ Estrutura de Entidades

### Product
```csharp
- Id: int
- Name: string
- Description: string?
- Price: decimal
- Stock: int
- IsActive: bool
- CreatedAt: DateTime
- UpdatedAt: DateTime?
- CreatedByUserId: int
- CreatedBy: User?
- PendingDeletion: bool
- RequestedDeletionByUserId: int?
- RequestedDeletionBy: User?
- DeletionRequestedAt: DateTime?
```

### UserProduct (Compras)
```csharp
- Id: int
- UserId: int
- ProductId: int
- Quantity: int
- PurchasePrice: decimal
- PurchasedAt: DateTime
```

## 📡 Endpoints da API

### Produtos - Visualização
```http
GET /api/product
GET /api/product/{id}
```
**Permissão**: `UsuarioOrAbove` (todos autenticados)

### Produtos - Criação
```http
POST /api/product
Body: {
  "name": "string",
  "description": "string",
  "price": decimal,
  "stock": int
}
```
**Permissão**: `FuncionarioOrAbove` (Level 2+)

### Produtos - Atualização
```http
PUT /api/product/{id}
Body: {
  "name": "string?",
  "description": "string?",
  "price": decimal?,
  "stock": int?,
  "isActive": bool?
}
```
**Permissão**: `FuncionarioOrAbove` (Level 2+)

### Produtos - Deleção
```http
DELETE /api/product/{id}
```
**Permissão**: `FuncionarioOrAbove` (Level 2+)
- **Funcionário**: Marca como `PendingDeletion = true`
- **Admin**: Deleta imediatamente do banco

### Aprovação de Deleção (Admin)
```http
GET /api/product/pending-deletion
POST /api/product/{id}/approve-deletion
POST /api/product/{id}/reject-deletion
```
**Permissão**: `AdminOnly` (Level 3)

### Compra de Produtos
```http
POST /api/product/purchase
Body: {
  "productId": int,
  "quantity": int
}
```
**Permissão**: `UsuarioOrAbove` (todos autenticados)

**Validações**:
- Produto deve estar ativo (`IsActive = true`)
- Produto não pode estar pendente de deleção
- Estoque deve ser suficiente
- Desconta do estoque automaticamente

### Minhas Compras
```http
GET /api/product/my-products
```
**Permissão**: `UsuarioOrAbove` (todos autenticados)

## 🔄 Fluxo de Deleção com Aprovação

### Cenário 1: Funcionário solicita deleção
1. Funcionário faz `DELETE /api/product/{id}`
2. Sistema marca produto como `PendingDeletion = true`
3. Registra quem solicitou e quando
4. Retorna mensagem: "Solicitação de deleção enviada para aprovação do admin"

### Cenário 2: Admin visualiza e aprova
1. Admin consulta `GET /api/product/pending-deletion`
2. Admin decide aprovar `POST /api/product/{id}/approve-deletion`
3. Sistema deleta o produto permanentemente
4. Retorna: "Deleção aprovada e produto removido"

### Cenário 3: Admin rejeita
1. Admin decide rejeitar `POST /api/product/{id}/reject-deletion`
2. Sistema remove flags de deleção pendente
3. Produto volta ao estado normal
4. Retorna: "Solicitação de deleção rejeitada"

### Cenário 4: Admin deleta diretamente
1. Admin faz `DELETE /api/product/{id}`
2. Sistema deleta imediatamente (sem pendência)
3. Retorna: "Produto deletado com sucesso"

## 🛡️ Implementação de Segurança

### Validação em Múltiplas Camadas

#### 1. Controller (Atributos)
```csharp
[Authorize(Policy = "FuncionarioOrAbove")]
public async Task<IActionResult> Create(...)
```

#### 2. Service (Programática)
```csharp
if (!await IsUserLevelAsync(user, 2))
{
    return (false, "Apenas funcionários ou admins...");
}
```

#### 3. Business Rules
- Produto pendente de deleção não pode ser editado
- Produto inativo não pode ser comprado
- Validação de estoque antes da compra
- Registro de auditoria (quem criou, quem solicitou deleção)

## 💼 Casos de Uso Implementados

### UC001 - Listar Produtos
**Ator**: Qualquer usuário autenticado  
**Fluxo**:
1. Usuário solicita lista de produtos
2. Sistema retorna produtos ativos (não pendentes)
3. Admin vê também inativos e pendentes

### UC002 - Criar Produto
**Ator**: Funcionário ou Admin  
**Fluxo**:
1. Funcionário/Admin preenche dados
2. Sistema valida permissão (Level >= 2)
3. Sistema cria produto e associa ao criador
4. Retorna produto criado

### UC003 - Atualizar Produto
**Ator**: Funcionário ou Admin  
**Fluxo**:
1. Funcionário/Admin envia dados atualizados
2. Sistema valida permissão
3. Sistema verifica se não está pendente de deleção
4. Atualiza produto e registra `UpdatedAt`

### UC004 - Deletar Produto (Funcionário)
**Ator**: Funcionário  
**Fluxo**:
1. Funcionário solicita deleção
2. Sistema marca `PendingDeletion = true`
3. Registra solicitante e data
4. Aguarda aprovação do Admin

### UC005 - Deletar Produto (Admin)
**Ator**: Admin  
**Fluxo**:
1. Admin solicita deleção
2. Sistema detecta nível Admin
3. Remove produto imediatamente
4. Sem necessidade de aprovação

### UC006 - Aprovar Deleção
**Ator**: Admin  
**Fluxo**:
1. Admin visualiza pendências
2. Admin aprova deleção específica
3. Sistema remove produto permanentemente

### UC007 - Comprar Produto
**Ator**: Qualquer usuário autenticado  
**Fluxo**:
1. Usuário seleciona produto e quantidade
2. Sistema valida:
   - Produto ativo
   - Não pendente de deleção
   - Estoque suficiente
3. Sistema:
   - Reduz estoque
   - Cria registro em UserProduct
   - Registra preço de compra (histórico)
4. Retorna confirmação

### UC008 - Ver Minhas Compras
**Ator**: Qualquer usuário autenticado  
**Fluxo**:
1. Usuário solicita histórico
2. Sistema busca compras do usuário
3. Retorna lista com detalhes

## 🧪 Testando o Sistema

Use o arquivo `ProductCatalog.http` incluído no projeto que contém exemplos completos de todas as operações.

### Sequência recomendada:
1. Registrar 3 usuários (Admin, Funcionário, Usuario)
2. Fazer login e copiar tokens
3. Criar produtos como Funcionário/Admin
4. Tentar criar como Usuario (deve falhar)
5. Testar deleção como Funcionário (pendente)
6. Aprovar/Rejeitar como Admin
7. Comprar produtos como Usuario
8. Ver histórico de compras

## 📂 Arquivos Principais

### Domain Layer
- `Entities/Product.cs` - Entidade produto com auditoria
- `Entities/UserProduct.cs` - Relacionamento compras
- `Interfaces/IProductRepository.cs` - Contrato do repositório

### Application Layer
- `Services/ProductService.cs` - Lógica de negócio completa
- `DTOs/ProductDto.cs` - DTOs request/response

### Infrastructure Layer
- `Repositories/ProductRepository.cs` - Acesso a dados
- `Context/AppDbContext.cs` - Configuração EF Core

### API Layer
- `Controllers/ProductController.cs` - Endpoints REST
- `ProductCatalog.http` - Exemplos de requisições

## 🚀 Características Técnicas

✅ Clean Architecture  
✅ Repository Pattern  
✅ DTO Pattern  
✅ Authorization Policies  
✅ Auditoria (CreatedBy, timestamps)  
✅ Soft Delete (PendingDeletion)  
✅ Workflow de aprovação  
✅ Validações em camadas  
✅ Controle de estoque  
✅ Histórico de compras  
✅ Preço congelado na compra  

## 📊 Diagrama de Permissões

```
Admin (Level 3)
  ├─ CRUD completo
  ├─ Deleta imediatamente
  ├─ Aprova/Rejeita deleções
  └─ Vê produtos inativos
  
Funcionario (Level 2)
  ├─ Criar produtos
  ├─ Editar produtos  
  ├─ Solicitar deleção (pendente)
  ├─ Visualizar produtos
  └─ Comprar produtos
  
Usuario (Level 1)
  ├─ Visualizar produtos
  ├─ Comprar produtos
  └─ Ver histórico compras
```

## 🔐 Segurança Implementada

1. **JWT Authentication** - Tokens assinados
2. **Authorization Policies** - Baseadas em níveis
3. **Claim-based Security** - role_level no token
4. **Validação dupla** - Controller + Service
5. **Auditoria** - Registro de quem fez o quê
6. **Workflow controlado** - Deleção requer aprovação
7. **Restrições de negócio** - Ex: não editar pendente de deleção

Sistema completo e pronto para produção! 🎉

