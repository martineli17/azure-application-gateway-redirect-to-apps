# Azure Application Gateway

O Azure Application Gateway é uma alternativa para centralizar, entre outras coisas, o redirecionamento de solicitações para os serviços específicos, abstraindo assim a necessidade de conhecer o endpoint (IP) original de cada um deles e permitindo a adição de novos mais facilmente.
Antes de iniciar a explicação prática de como pode ser adicionado application gateway da Azure, vale entender alguns dos recursos utilizados por ele:
- `Public IP`: o gateway utiliza de um IP público registrado. Será através deste IP que ele ouvirá as requisições e, com isso, redirecioná-las de acordo com a necessidade especificada.
- `Listener`: é a especificação de uma regra para 'ouvir' as requisições do IP Público registrado a acima. Cada Listener precisa ser registrado em uma porta e não pode haver mais de um ouvindo na mesma.
- `Backend Pool`: é o registro de um conjunto de uma ou mais aplicações que ficarão agrupadas para atender uma determinada regra de roteamento. Caso um backend pool tenha mais de uma aplicação registrada, será feito o Loadbalancer entre elas para atender as solicitações.
- `Backend Settings`: é onde é feita algumas configurações a respeito do roteamento. Aqui, por exemplo, definimos qual será a porta da aplicação do backend pool que desejamos utilizar e qual será o host de redirecionamento, podendo ser uma URL específica ou o próprio host do backend pool selecionado. Além disso, definimos se há a necessidade de [sobreescrever a rota original](https://learn.microsoft.com/en-us/azure/application-gateway/configuration-http-settings#override-backend-path).
- `Rules`: aqui é feito as regras de redirecionamento, mais específico sobre os paths da URL que desejamos configurar, para qual backend pool e qual backend settings.
- `Probes`: é responsável por verificar a saúde de cada uma das aplicações registradas no backend pool, validando se as mesmas estão disponíveis para receberem as solicitações.

## Implementação
Neste exemplo, para implementar o Azure Application Gateway, será necessário alguns recursos:
- Virtual Network
  > Normalmente, quando um Gateway é implementado, um dos objetivos é evitar que determinadas aplicações tenha um IP Público exposto, utilizando o próprio Gateway como porta de entrada.
  > Aqui, a virtual network é utilizada para integrar as aplicações e o gateway em uma mesma rede
- Azure Web App
  > Foi implementado 3 Web Apps: um frontend e dois backends, a fim de simular os redirecionamentos.
- Azure Application Gateway

## Criando a Virtual Network
![create_virtual_network](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/393c688f-7007-4005-bf2f-bde477821ee0)

- Foi criada duas subnets: uma para as aplicações e outra para o gateway. O gateway precisa de uma subnet específica, visto que ele tem a opção de autoscale, sendo assim ele pode precisar de adicionar mais IPs na subnet alocada.

## Criando os Web Apps
![create_web_app](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/f4cdb514-e80e-4d95-b53e-80df1e979ec9)

- No GIF acima, foi criado somente um Web App
  - A opção de deploy habilitada foi do tipo container
  - Na parte de network, foi desabilitado a opção de IP Publico, visto que não há necessidade.
  - Foi habilitada a injeção de Virtual Networks, informando qual seria o nome para o private endpoint na rede (nesse caso selecionando o próprio DNS da Azure) e qual a subnet. Note que a subnet selecionada foi a `SUBNET-HOSTS`
 - Observação:
   > Crie os outros dois Web Apps para os backends.
   > Após a criação, desabilite a obrigatoriedade do HTTPs. Nesse exemplo não foi adicionado certificado SSL.

## Criando Azure Applicaiton Gateway
![create_gt_basic](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/3b91eb9f-0ce0-485a-adde-8820cf1a1ac6)

- Nas informações básicas para a criação do gateway, é necessário informar se o autoscale será habilitado ou não. Caso não seja, informar a quantidade de réplicas.
- No final, é necessário informar as informações sobre o network, adicionando a Virtual Network (que será a mesma dos hosts) e a SUBNET que será a específica para o gateway: `SUBNET-GT`

![create_gt_front](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/0ef23373-fb17-4f95-bf7f-62938eff3183)

- Na próxima etapa, é necessário associar um Public IP para o gateway. Caso não tenha nenhum criado, pode-se criar um novo no momento.

![create_gt_backendpool](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/6abf5b85-a3f4-43ea-b96d-909a69fbf607)

- A próxima etapa é para informar os backend pools que serão utilizados. Nesse cenário, é necessário criar um backend pool para cada aplicação a fim de realizar o roteamento para elas.
- Tem a opção de gerenciar os pools após o criação do gateway, caso não queira adicionar todos no momento.

![create_gt_listener](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/f62afdf9-dd86-4bd1-aacc-218a5a656851)

- A etapa final é realizar a configuração de roteamento. Inicialmente, precisa-se criar uma rule e um listener, informando qual é a porta que este listener irá monitorar.

![create_gt_settings](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/b17594b1-9c45-4a8e-804f-84c87339cddb)

- Após a criação da rule e do listener, é necessário configurar os paths de roteamento. Inicialmente, foi adicionado um novo pool chamado `pool-default` e um settings chamado `settings-default`, somente para deixá-los como configuração padrão.
- Existe a opção de adicionar roteamento de acordo com os paths da URL. Nessa definição, é necessário adicionar um settings, qual será o path de redirecionamento e qual o backend pool responsável.
  - Foi criado um path `/frontend/*`
  - Foi criado um path `/backend01/*`
  - Foi criado um path `/backend02/*`
 - Feito essas configurações, basta criar o gateway.

## Configurações após a criação
### Criando um DNS para o gateway
Para criar um DNS da azure para o gateway, basta:
1. Acessar o Public IP que está relacionado ao gateway
2. Acessar a aba de `Configuration`
3. No campo `DNS name label`, adicionar a label desejada
Para este exemplo, o DNS ficou `http://estudos-gt.eastus.cloudapp.azure.com`

### Executando a pipeline do frontend e backend para publicação
As pipelines para publicar a aplicação do frontend, backend 01 e backend 02 podem ser executadas.
- `Pipeline do frontend`
  > Será necessário informar qual é o IP ou DNS (adcionado anteriormente) do application gateway e também o nome do webapp criado.
  > A variável `PUBLIC_IP` será configurada para o DNS do gateway, a fim de obter os arquivos corretamente (main.js, main.css, etc)
- `Pipeline do backend`
  > Será necessário informar qual é o nome do webapp criado.

### Configurações adicionais
Temos a possibilidade de gerenciar, entre outras, algumas informações do gateway:
- Os backend pool
- Os backend settings
- Rules de redirecionamento
- Probes para verificação de integridade das aplicações

Algumas configurações adicionais a serem feitas
- Validar o override path nos `Backend Settings` criados. Validar se o override está correto (para este cenário seria para sobreescrever toda o path da regra: `/`)
- Sobreescrever o Host Name através do backend pool
- Configurar Probe para as aplicações do backend, informando um endpoint válido para a verificação de integridade da aplicação. Para este exemplo, o `/swagger` pode ser utilizado.

![add_probe](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/f30acb20-3295-42fd-8852-5e07bbb62c46)

Feitas as configurações, as probes podem ser verificadas:

![image](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/52a0892d-9bb0-4635-a4a9-cfe4725dd33d)

## Resultado final
Agora as requisições estão sendo gerenciadas pelo gateway e redirecionadas para o serviço específico.

![exemplo](https://github.com/martineli17/azure-application-gateway-redirect-to-apps/assets/50757499/dcba2f9a-e3b8-41b2-b226-a94258518c8f)



