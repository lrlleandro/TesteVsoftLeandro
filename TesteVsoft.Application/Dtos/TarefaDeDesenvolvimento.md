# Tarefa de Desenvolvimento para Desenvolvedor Fullstack Sênior

## Descrição da Tarefa
O candidato deverá desenvolver uma pequena aplicação que simule um sistema de gerenciamento de tarefas. A aplicação deverá conter as seguintes funcionalidades:
● Criação, listagem, atualização e exclusão de tarefas.
● Atribuição de tarefas a usuários.
● O usuário deverá ser notificado quando uma nova tarefa lhe foi atribuída.
● Apenas usuários autenticados poderão acessar a aplicação.

## Requisitos Técnicos
O candidato deverá demonstrar proficiência em:
● Desenvolvimento de aplicações utilizando .NET 8.
● Desenvolvimento de front-end utilizando Vue.js.
  ○ Deve possuir gerenciamento e compartilhamento de estado entre componentes usando alguma store library de sua escolha.
● Implementação utilizando princípios como Clean Code, KISS, YAGNI e DRY.
● Utilização de RabbitMQ para o envio de atualizações/notificações para o usuário.
● Criação de testes unitários.
● Criação de testes de integração para pelo menos 1 endpoint.
● CRUD para “Taskˮ
  ○ Id (Guid), Title, Description, DueDate, Status (Pending, InProgress, Completed)
  ○ A “Taskˮ será criada e atribuída por padrão ao usuário logado.
● Um endpoint para inclusão de usuários deverá ser disponibilizado onde será definido uma quantidade de usuários e uma máscara para o nome de usuário.
  ○ POST /users/createRandom { “amount": 1000, “userNameMaskˮ: “user_{{random}}ˮ }
  ○ O texto “{{random}}ˮ será substituído por uma parte randômica de sua escolha.

## Tecnologias
1. .NET 8 Web Api.
2. Vue.js
3. Utilizar RabbitMQ para atualizações/notificações de usuário.
4. Escrever testes unitários e de integração para garantir a qualidade do código.
5. Documentar o código e a arquitetura da aplicação.

## Critérios de Avaliação
Os candidatos serão avaliados com base nos seguintes critérios:
● Qualidade do código e da arquitetura.
● Utilização das melhores práticas de desenvolvimento.
● Cobertura de testes.
● Documentação do código e da arquitetura.
● Funcionalidade da aplicação.

## Entrega
O candidato deverá entregar um repositório Git com o código fonte da aplicação, juntamente com a documentação e os testes. Deverá ser possível acessar a aplicação (frontend e backend) utilizando uma simples linha de comando como “docker compose up -dˮ.

## Bônus
Possibilidade de integração com outras ferramentas (Outlook, Thunderbird) através do link iCalendar.