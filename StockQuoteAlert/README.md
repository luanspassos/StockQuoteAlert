# Stock Quote Alert

## Como funciona

O programa executa continuamente os seguintes passos:

1. Busca a cotação atual do ativo via a API brapi.dev
2. Compara o preço atual com os preços de referência informados
3. Envia um e-mail caso:
   - o preço esteja acima do limite de venda
   - o preço esteja abaixo do limite de compra
4. Aguarda um intervalo configurável e repete o processo

Para evitar múltiplos alertas repetidos, o sistema só envia um novo e-mail quando há mudança de estado.

## Tecnologias utilizadas

- C# (.NET 8)
- HTTP Client (consumo de API)
- SMTP (envio de e-mail)

## Requisitos

- .NET 8 ou superior
- Conta Gmail (ou outro SMTP)

## Configuração

1. Copie `appsettings.example.json` para `appsettings.json`
2. Preencha com seus dados:
   - Email de destino
   - Email de usuario (remetente)
   - Senha de app do Gmail

	O Gmail não aceita sua senha normal. Você precisa criar uma "Senha de App":

	1.1 Acesse: https://myaccount.google.com/apppasswords
	1.2 Faça login com sua conta Google
	1.3 Em **"Selecione o app"** → escolha **"Outro (nome personalizado)"**
	1.4 Digite: `StockQuoteAlert`
	1.5 Clique em **"Gerar"**
	1.6 **Copie a senha gerada** (ex: `abcd efgh ijkl mnop`)

3. Ajuste o intervalo de verificação (padrão: 30 segundos)

## Como usar

Se você estiver na pasta pai, por exemplo:

PS C:\Users\User\source\repos\StockQuoteAlert>

entre na pasta do projeto com:
```bash
cd StockQuoteAlert
```
Depois, execute em PS C:\Users\User\source\repos\StockQuoteAlert>:

```bash
dotnet run -- <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
```
Ex: dotnet run -- PETR4 22.67 22.59
