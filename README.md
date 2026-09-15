# SharpMinded API

**[Status: Desenvolvimento Ativo / Fase MVP]**  
Uma API RESTful robusta, segura e escalável construída para uma plataforma de aprendizado baseada em **repetição espaçada** (flashcards). O sistema é projetado para otimizar a retenção de conhecimento através do gerenciamento estruturado de baralhos (decks) e cartões de estudo.

> **💡 Nota para Recrutadores e Gestores Técnicos:**  
> Este projeto demonstra a aplicação prática de desenvolvimento backend moderno com .NET, autenticação segura, princípios de arquitetura limpa e integração com serviços externos (Supabase). Os módulos principais de autenticação e gerenciamento de decks estão concluídos, com a lógica de agendamento de repetição espaçada em desenvolvimento.

## 🚀 Funcionalidades Principais
- ✅ **Autenticação Segura**: Autenticação baseada em JWT integrada ao Supabase Auth.
- ✅ **Gerenciamento de Decks e Cards**: Endpoints RESTful para operações CRUD (Criar, Ler, Atualizar, Deletar) utilizando DTOs para validação e segurança.
- ✅ **Arquitetura Limpa**: Separação clara de responsabilidades usando Models, DTOs e Controllers, garantindo código manutenível e testável.
- ✅ **Documentação da API**: Especificações OpenAPI (Swagger) geradas automaticamente para facilitar a integração com o frontend e testes.
- 🚧 **Lógica de Repetição Espaçada**: *(Em progresso)* Implementação de algoritmos de agendamento (ex: SM-2) para otimizar os intervalos de revisão dos cartões.
- 🚧 **Integração Persistente**: *(Em progresso)* Configuração avançada de Entity Framework Core com o PostgreSQL do Supabase.

## 🛠️ Tech Stack
- **Linguagem & Framework**: C# 12 / .NET 10
- **Banco de Dados & Backend**: Supabase (PostgreSQL), Entity Framework Core
- **Autenticação**: JWT Bearer Tokens, Supabase Auth
- **API & Documentação**: ASP.NET Core Web API, OpenAPI / Swagger
- **Configuração**: `DotNetEnv` para gerenciamento seguro de variáveis de ambiente

## 🏗️ Arquitetura e Boas Práticas
- **Data Transfer Objects (DTOs)**: Utilizados para desacoplar os modelos de domínio internos dos contratos da API, evitando *over-posting* e garantindo flexibilidade.
- **Injeção de Dependência**: Uso do contêiner nativo do .NET para gerenciamento de ciclo de vida de serviços (ex: cliente Supabase).
- **Segurança em Primeiro Lugar**: Redirecionamento HTTPS, middlewares rigorosos de autenticação/autorização e manipulação segura de chaves de serviço via variáveis de ambiente.

## 📂 Estrutura do Projeto
```text
SharpMinded/
├── Controllers/     # Endpoints da API (ex: AuthController, DecksController)
├── DTOs/            # Objetos de Transferência de Dados para validação de requisição/resposta
├── Models/          # Entidades de domínio e mapeamentos de banco de dados
├── Properties/      # Configurações de inicialização (launchSettings)
├── Program.cs       # Ponto de entrada da aplicação e configuração de Injeção de Dependência
└── appsettings.json # Configurações específicas do ambiente
```

## ⚙️ Como Executar Localmente
1. **Clone o repositório**:
   ```bash
   git clone https://github.com/CoininDev/SharpMinded.git
   cd SharpMinded
   ```
2. **Configure as Variáveis de Ambiente**:  
   Crie um arquivo `.env` na raiz do projeto com suas credenciais do Supabase:
   ```env
   SUPABASE_URL=sua_url_do_projeto_supabase
   SUPABASE_KEY=sua_chave_de_servico_supabase
   ```
3. **Restaure as dependências e execute**:
   ```bash
   dotnet restore
   dotnet run
   ```
4. **Acesse a Documentação da API**:  
   Navegue até o endpoint do Swagger/OpenAPI (ex: `https://localhost:<porta>/swagger` ou `/openapi`) para explorar e testar os endpoints interativamente.

## 🎯 Roadmap e Próximos Passos
- [ ] Implementar o algoritmo central de agendamento de repetição espaçada.
- [ ] Adicionar testes unitários e de integração abrangentes (xUnit/NUnit).
- [ ] Refinar configurações do Entity Framework Core e migrations de banco de dados.
- [ ] Implementar *Rate Limiting* e middleware avançado de tratamento de erros globais.

---
*Desenvolvido com ❤️ utilizando .NET*
