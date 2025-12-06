# Documentação do Projeto: Sistema de Gestão de Corridas - RaceUfla

## 1. Contextualização Acadêmica
Este software foi desenvolvido como instrumento avaliativo da disciplina de **Programação Orientada a Objetos (POO)**, integrante da grade curricular do curso de **Bacharelado Interdisciplinar em Ciência e Tecnologia (BICT)** da **Universidade Federal de Lavras (UFLA) - Câmpus Paraíso**.

O projeto visa demonstrar a aplicação prática de conceitos fundamentais da orientação a objetos, como herança, polimorfismo, encapsulamento, abstração e composição, utilizando a plataforma .NET e o padrão arquitetural MVC.

## 2. Objetivo do Sistema
O **RaceUfla** tem como intuito informatizar o ciclo de vida de eventos esportivos de corridas de rua. O sistema atua como uma plataforma intermediária que conecta:
* **Organizadores:** Entidades responsáveis por criar eventos, definir kits de participação e gerenciar a lista de inscritos.
* **Corredores:** Atletas que buscam eventos, realizam inscrições e acompanham seu status de participação.

## 3. Processo de Desenvolvimento e Evolução
O desenvolvimento do sistema seguiu uma abordagem incremental.

**Requisitos Iniciais:**
A proposta original consistia em um sistema básico de cadastro de usuários (Corredor e Organizador) e cadastro de corridas simples, com foco apenas na persistência de dados.

**Evolução e Regras de Negócio Adicionadas:**
Durante a análise e implementação, foram identificadas lacunas de negócio que exigiram a adição de funcionalidades complexas para garantir a integridade e usabilidade do sistema:

1.  **Lógica de Inscrição Híbrida:** Implementou-se a regra de negócio onde toda corrida deve possuir, obrigatoriamente, uma opção de "Inscrição Básica/Gratuita", independentemente dos kits pagos criados pelo organizador.
2.  **Gestão Financeira Simulada:** Adicionou-se a entidade de pagamento via PIX. O sistema diferencia automaticamente inscrições "Gratuitas" (aprovadas imediatamente) de inscrições "Pagas" (que nascem com status Pendente aguardando validação).
3.  **Composição de Kits:** A criação de kits evoluiu de um texto simples para uma seleção de itens (checkboxes), permitindo ao sistema identificar automaticamente se o kit possui camiseta e, consequentemente, exigir a seleção de tamanho no momento da inscrição.
4.  **Painel Administrativo:** Foi desenvolvida uma interface de relatório para o organizador, permitindo o cruzamento de dados entre Inscrições e Corredores, com funcionalidades de confirmação manual de pagamento e exclusão de participantes.

## 4. Arquitetura Técnica e Estrutura de Código

O projeto foi desenvolvido em **C#** utilizando o framework **ASP.NET Core MVC (.NET 8.0)**. A persistência de dados é realizada em memória (Singleton Pattern via classe estática) para fins de demonstração acadêmica.

### 4.1. Classes e Relacionamentos (Models)

A modelagem das classes reflete os pilares da POO:

* **`Usuario` (Classe Abstrata):**
    * **Conceito:** Generalização. Impede a criação de um usuário genérico.
    * **Atributos:** `Id`, `Nome`, `Email`, `Senha`, `Telefone` e dados de Endereço (`Logradouro`, `Cidade`, `UF`).
    * **Relacionamento:** Classe base para `Corredor` e `Organizador`.

* **`Corredor` (Herda de Usuario):**
    * **Conceito:** Especialização.
    * **Atributos:** `CPF`, `RG`, `Sexo`, `DataNascimento`.

* **`Organizador` (Herda de Usuario):**
    * **Conceito:** Especialização.
    * **Atributos:** `CpfCnpj`, `ChavePix`.

* **`Corrida`:**
    * **Atributos:** `Id`, `Nome`, `DataInicio`, `HorarioLargada`, `CaminhoImagemMapa`, `KitsSelecionados` (Lista de IDs).
    * **Relacionamento:** Possui uma associação direta com `Organizador` (dono do evento) e uma agregação de `Kits`.

* **`Kit`:**
    * **Atributos:** `Nome`, `Preco`, `ItensTexto`, `TemCamisa` (Booleano).
    * **Regra:** Define a estrutura do produto vendido. O atributo `TemCamisa` é crucial para renderização condicional na View.

* **`Inscricao`:**
    * **Conceito:** Classe Associativa. Resolve a relação N:N entre `Corredor` e `Corrida`.
    * **Atributos:** `Status` (Pendente/Confirmado), `ValorPago`, `TamanhoCamisa`, `KitId`, `CorredorId`, `CorridaId`.

### 4.2. Métodos Principais e Lógica de Controle

Abaixo estão descritos os métodos mais relevantes que contêm a lógica de negócio do sistema.

#### **No `HomeController` (Área Pública/Corredor):**

* **`Detalhes(int id)`:**
    * **Função:** Prepara a tela de inscrição.
    * **Lógica:** Recupera a corrida e cruza os dados para buscar os kits pertencentes ao organizador responsável.
    * **Regra Crítica:** Instancia dinamicamente um objeto `Kit` fictício (Inscrição Básica Gratuita) e o insere no topo da lista de opções, garantindo que sempre haja uma modalidade de participação gratuita.

* **`Inscrever(int corridaId, int kitId, ...)`:**
    * **Função:** Processa o registro do atleta.
    * **Lógica:** Verifica se o `kitId` corresponde ao kit gratuito ou pago.
    * **Automação:** Se o valor for R$ 0,00, define o status como "Confirmado". Se for maior que zero, define como "Pendente".

* **`CancelarInscricao(int id)`:**
    * **Função:** Permite ao corredor desistir da prova.
    * **Segurança:** Valida se a inscrição pertence realmente ao usuário logado na sessão antes de remover o registro.

#### **No `OrganizadorController` (Área Administrativa):**

* **`CriarKit(Kit model, List<string> itensSelecionados, ...)`:**
    * **Função:** Cadastra um novo produto.
    * **Lógica:** Recebe uma lista de strings dos checkboxes da View. Percorre essa lista para verificar a existência da string "Camisa" ou "Camiseta". Se encontrada, define a propriedade `TemCamisa = true`, ativando a lógica de tamanhos no frontend.

* **`ConfirmarPagamento(int idInscricao)`:**
    * **Função:** Validação financeira manual.
    * **Lógica:** Localiza a inscrição e altera seu status para "Confirmado".
    * **Segurança:** Verifica se o organizador logado é, de fato, o dono da corrida à qual a inscrição pertence antes de permitir a alteração.

* **`InformacoesCorrida(int id)`:**
    * **Função:** Gera o relatório gerencial.
    * **Lógica:** Realiza uma consulta LINQ filtrando todas as inscrições onde `CorridaId` corresponde ao evento atual, cruzando dados para exibir nomes e contatos dos corredores.

## 5. Instruções de Execução

Para executar a aplicação corretamente em ambiente local:

1.  **Pré-requisitos:**
    * Visual Studio 2022 (com carga de trabalho ASP.NET e desenvolvimento Web).
    * .NET SDK 8.0 instalado.

2.  **Passos:**
    * Clone o repositório ou extraia os arquivos do projeto.
    * Abra o arquivo de solução `CorridaUfla2.sln` no Visual Studio.
    * Realize a compilação da solução (Build > Build Solution) para restaurar as dependências do NuGet.
    * Execute a aplicação pressionando `F5` ou configurando o perfil de execução para `http` / `IIS Express`.

3.  **Nota sobre Persistência:**
    * O sistema utiliza persistência em memória volátil (`DadosApp.cs`). Ao interromper a execução do servidor (Stop Debugging), todos os dados cadastrados (usuários, corridas e kits) serão reiniciados.

## 6. Autoria e Contato

Projeto desenvolvido por **Higor Felipe dos Reis**.

* **E-mail Institucional:** higor.reis@estudante.ufla.br
* **E-mail Pessoal:** hllgorfellpedosrells@gmail.com
