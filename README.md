# Projeto teste para Datarisk

Este e um projeto desenvolvido para a Datarisk.

## Passos iniciais

Executar arquivos .bat para Windows e .sh para Linux

Executar o comando <code>docker compose up --build -d</code> na pasta do respositorio.  
Apos o termino da execucao do script, a API deve estar rodando na porta 7000, para ter acesso a documentacao
acessar a rota <http://localhost:7000/swagger/index.html>

## Documentacao

Modelo base de resposta:  
```
{  
  "Information": "Qualquer informacao adicional",  
  "Data": "Qualquer tipo de dado de resposta (nao precisa ser necessariamente uma string), pode ser nulo",  
  "Errors": ["Lista com erros durante execucao", "Pode ser nulo"],  
  "Identifier": "Identificador da requisicao"  
}
```

### Execute [POST]

Executa uma funcao de um script com dados parametros.

Response: Id para consulta em 'Execute/Status/Id' ou 'Execute/Id'

### Execute/Status/<code>Id</code> [GET]

Id: Id de execucao  
Verifica o status de execucao do script.

### Execute/<code>Id</code> [GET]

Id: Id de execucao  
Retorna a resposta de uma execucao finalizada.

### Script/<code>Nome</code> [POST]

Nome: Nome para registro do script.  
Registra um script para analise.

### Script/<code>Nome</code> [GET]

Nome: Nome de registro do script.  
Retorna o script do sistema.

### Script [GET]

Lista todos os scripts.

### Script/Status/<code>Nome</code> [GET]

Nome: Nome de registro do script.  
Verifica o status de analise do script.

## Teste

Cada arquivo JSON na pasta 'Test' tem o body para ser executado.  
Certifcar-se que o arquivo 'teste' esta no sistema, caso nao, executar o script.(bat|sh).

### Teste 1
Trata-se de uma sequencia fibonacci, onde deve ser retornado o elemento 10 da sequencia.  
Resultado: 55

### Teste 2
Trata-se de uma formula de bhaskara com os valores (1, 2, 3).  
Resultado: Nao possui raizes reais.

### Teste 3
Trata-se de uma formula de bhaskara com os valores (1, 2, -15).  
Resultado:
```
{  
"I": 3,  
"II": -5  
}
```

### Teste 4
Trata-se de uma analise de dados de uma lista, arquivo data.json.

## Questionario

### Como voce faria para lidar com grandes volumes de dados enviados para pre-processamento? O design atual da API e suficiente?

Para lidar com grande volume de dados, foi desenvolvido um design para API de recebimento de dados e devolucao
do processamento somente. O processamento e analise de script foi delegada a uma aplicacao separada, evitando
que a API sofra com interrupcoes por excesso de processamento.

### Que medidas voce implementaria para se certificar que a aplicacao nao execute scripts maliciosos?

Com a ajuda da biblioteca Jint, e feita um leitura do codigo, transformando o texto em nodes que podem
assim ser checado os tipos e nomes, sendo assim feita uma busca por palavras chaves e tipos que nao sao permitidas
para a execucao.

### Como aprimorar a implementacao para suportar um alto volume de execucoes concorrentes de scripts?

Para a alta demanda de execucoes, ter um programa separado para o trabalho de processamento, que possa
ser escalavel e que execute os scripts em lotes de forma assincrona, ajuda na performance.
Uma fila para espera na liberacao de espaco para processamento.

### Como voce evoluiria a API para suportar o versionamento de scripts?

Para um versionamento da API, pode ser utilizado na rota da chamada a introducao de um versionador,
ex: www.api.com/v1/caminho ou www.api.com/v2/caminho, sendo o v1 e v2 a versao a ser usada na chamada.
Pode tambem ser inserido dentro de um header, ou dentro do objeto a ser passado para a API.

### Que tipo de politica de backup de dados voce aplicaria neste cenario?

Backup das informacoes do banco de dados, e caso exista, um backup do volume onde fica salvo os scripts.

### Como voce enxerga o paradigma funcional beneficiando a solucao deste problema?

Validacao de modelos de requisicao, LINQs para consulta no banco de dados.
