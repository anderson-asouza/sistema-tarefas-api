# Documentation for Sistema API

<a name="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

All URIs are relative to *http://localhost*

| Class | Method | HTTP request | Description |
|------------ | ------------- | ------------- | -------------|
| *FlagsApi* | [**apiFlagsGet**](Apis/FlagsApi.md#apiflagsget) | **GET** /api/Flags |  |
*FlagsApi* | [**apiFlagsIdDelete**](Apis/FlagsApi.md#apiflagsiddelete) | **DELETE** /api/Flags/{id} |  |
*FlagsApi* | [**apiFlagsIdGet**](Apis/FlagsApi.md#apiflagsidget) | **GET** /api/Flags/{id} |  |
*FlagsApi* | [**apiFlagsIdPut**](Apis/FlagsApi.md#apiflagsidput) | **PUT** /api/Flags/{id} |  |
*FlagsApi* | [**apiFlagsPost**](Apis/FlagsApi.md#apiflagspost) | **POST** /api/Flags |  |
| *ModelosTarefaApi* | [**apiModelosTarefaGet**](Apis/ModelosTarefaApi.md#apimodelostarefaget) | **GET** /api/ModelosTarefa |  |
*ModelosTarefaApi* | [**apiModelosTarefaIdModeloTarefaDelete**](Apis/ModelosTarefaApi.md#apimodelostarefaidmodelotarefadelete) | **DELETE** /api/ModelosTarefa/{idModeloTarefa} |  |
*ModelosTarefaApi* | [**apiModelosTarefaIdModeloTarefaGet**](Apis/ModelosTarefaApi.md#apimodelostarefaidmodelotarefaget) | **GET** /api/ModelosTarefa/{idModeloTarefa} |  |
*ModelosTarefaApi* | [**apiModelosTarefaIdModeloTarefaPut**](Apis/ModelosTarefaApi.md#apimodelostarefaidmodelotarefaput) | **PUT** /api/ModelosTarefa/{idModeloTarefa} |  |
*ModelosTarefaApi* | [**apiModelosTarefaPost**](Apis/ModelosTarefaApi.md#apimodelostarefapost) | **POST** /api/ModelosTarefa |  |
| *ModelosTramiteApi* | [**apiModelosTramiteGet**](Apis/ModelosTramiteApi.md#apimodelostramiteget) | **GET** /api/ModelosTramite |  |
*ModelosTramiteApi* | [**apiModelosTramiteIdModeloTramiteDelete**](Apis/ModelosTramiteApi.md#apimodelostramiteidmodelotramitedelete) | **DELETE** /api/ModelosTramite/{idModeloTramite} |  |
*ModelosTramiteApi* | [**apiModelosTramiteIdModeloTramiteGet**](Apis/ModelosTramiteApi.md#apimodelostramiteidmodelotramiteget) | **GET** /api/ModelosTramite/{idModeloTramite} |  |
*ModelosTramiteApi* | [**apiModelosTramiteIdModeloTramitePut**](Apis/ModelosTramiteApi.md#apimodelostramiteidmodelotramiteput) | **PUT** /api/ModelosTramite/{idModeloTramite} |  |
*ModelosTramiteApi* | [**apiModelosTramitePost**](Apis/ModelosTramiteApi.md#apimodelostramitepost) | **POST** /api/ModelosTramite |  |
| *TarefasApi* | [**apiTarefasAtivarDesativarIdTarefaPost**](Apis/TarefasApi.md#apitarefasativardesativaridtarefapost) | **POST** /api/Tarefas/ativar-desativar/{idTarefa} |  |
*TarefasApi* | [**apiTarefasGet**](Apis/TarefasApi.md#apitarefasget) | **GET** /api/Tarefas |  |
*TarefasApi* | [**apiTarefasIdTarefaDelete**](Apis/TarefasApi.md#apitarefasidtarefadelete) | **DELETE** /api/Tarefas/{idTarefa} |  |
*TarefasApi* | [**apiTarefasIdTarefaGet**](Apis/TarefasApi.md#apitarefasidtarefaget) | **GET** /api/Tarefas/{idTarefa} |  |
*TarefasApi* | [**apiTarefasIdTarefaPut**](Apis/TarefasApi.md#apitarefasidtarefaput) | **PUT** /api/Tarefas/{idTarefa} |  |
*TarefasApi* | [**apiTarefasMarcarFlagIdTarefaPost**](Apis/TarefasApi.md#apitarefasmarcarflagidtarefapost) | **POST** /api/Tarefas/marcar-flag/{idTarefa} |  |
*TarefasApi* | [**apiTarefasPost**](Apis/TarefasApi.md#apitarefaspost) | **POST** /api/Tarefas |  |
| *TramitesApi* | [**apiTramitesAssumirTramiteIdTramitePost**](Apis/TramitesApi.md#apitramitesassumirtramiteidtramitepost) | **POST** /api/Tramites/assumir-tramite/{idTramite} |  |
*TramitesApi* | [**apiTramitesBuscarTramitesResponsavelGet**](Apis/TramitesApi.md#apitramitesbuscartramitesresponsavelget) | **GET** /api/Tramites/buscar-tramites-responsavel |  |
*TramitesApi* | [**apiTramitesBuscarTramitesRevisorGet**](Apis/TramitesApi.md#apitramitesbuscartramitesrevisorget) | **GET** /api/Tramites/buscar-tramites-revisor |  |
*TramitesApi* | [**apiTramitesBuscarTramitesTramitadorGet**](Apis/TramitesApi.md#apitramitesbuscartramitestramitadorget) | **GET** /api/Tramites/buscar-tramites-tramitador |  |
*TramitesApi* | [**apiTramitesBuscarTramitesUsuarioGet**](Apis/TramitesApi.md#apitramitesbuscartramitesusuarioget) | **GET** /api/Tramites/buscar-tramites-usuario |  |
*TramitesApi* | [**apiTramitesComecarExecucaoTramiteIdTramitePost**](Apis/TramitesApi.md#apitramitescomecarexecucaotramiteidtramitepost) | **POST** /api/Tramites/comecar-execucao-tramite/{idTramite} |  |
*TramitesApi* | [**apiTramitesFinalizarExecucaoTramitePost**](Apis/TramitesApi.md#apitramitesfinalizarexecucaotramitepost) | **POST** /api/Tramites/finalizar-execucao-tramite |  |
*TramitesApi* | [**apiTramitesGet**](Apis/TramitesApi.md#apitramitesget) | **GET** /api/Tramites |  |
*TramitesApi* | [**apiTramitesIdTramiteGet**](Apis/TramitesApi.md#apitramitesidtramiteget) | **GET** /api/Tramites/{idTramite} |  |
*TramitesApi* | [**apiTramitesIncluirTramiteIdTarefaPost**](Apis/TramitesApi.md#apitramitesincluirtramiteidtarefapost) | **POST** /api/Tramites/incluir-tramite/{idTarefa} |  |
*TramitesApi* | [**apiTramitesRetrocederTramiteIdTramitePost**](Apis/TramitesApi.md#apitramitesretrocedertramiteidtramitepost) | **POST** /api/Tramites/retroceder-tramite/{idTramite} |  |
*TramitesApi* | [**apiTramitesRevisarTramiteAprovadoPost**](Apis/TramitesApi.md#apitramitesrevisartramiteaprovadopost) | **POST** /api/Tramites/revisar-tramite/{aprovado} |  |
| *UsuariosApi* | [**apiUsuariosAlterarSenhaPost**](Apis/UsuariosApi.md#apiusuariosalterarsenhapost) | **POST** /api/Usuarios/alterar-senha |  |
*UsuariosApi* | [**apiUsuariosGet**](Apis/UsuariosApi.md#apiusuariosget) | **GET** /api/Usuarios |  |
*UsuariosApi* | [**apiUsuariosIdUsuarioDelete**](Apis/UsuariosApi.md#apiusuariosidusuariodelete) | **DELETE** /api/Usuarios/{idUsuario} |  |
*UsuariosApi* | [**apiUsuariosIdUsuarioGet**](Apis/UsuariosApi.md#apiusuariosidusuarioget) | **GET** /api/Usuarios/{idUsuario} |  |
*UsuariosApi* | [**apiUsuariosIdUsuarioPut**](Apis/UsuariosApi.md#apiusuariosidusuarioput) | **PUT** /api/Usuarios/{idUsuario} |  |
*UsuariosApi* | [**apiUsuariosLogarPost**](Apis/UsuariosApi.md#apiusuarioslogarpost) | **POST** /api/Usuarios/logar |  |
*UsuariosApi* | [**apiUsuariosPost**](Apis/UsuariosApi.md#apiusuariospost) | **POST** /api/Usuarios |  |
*UsuariosApi* | [**apiUsuariosRemoverImagemPerfilDelete**](Apis/UsuariosApi.md#apiusuariosremoverimagemperfildelete) | **DELETE** /api/Usuarios/remover-imagem-perfil |  |
*UsuariosApi* | [**apiUsuariosUploadImagemPerfilPost**](Apis/UsuariosApi.md#apiusuariosuploadimagemperfilpost) | **POST** /api/Usuarios/upload-imagem-perfil |  |


<a name="documentation-for-models"></a>
## Documentation for Models

 - [FlagRequest](./Models/FlagRequest.md)
 - [FlagResponse](./Models/FlagResponse.md)
 - [ModeloTarefaRequest](./Models/ModeloTarefaRequest.md)
 - [ModeloTarefaResponse](./Models/ModeloTarefaResponse.md)
 - [ModeloTramiteRequest](./Models/ModeloTramiteRequest.md)
 - [ModeloTramiteResponse](./Models/ModeloTramiteResponse.md)
 - [ModeloTramiteUpdRequest](./Models/ModeloTramiteUpdRequest.md)
 - [ResponseCode](./Models/ResponseCode.md)
 - [ResponseModel](./Models/ResponseModel.md)
 - [TarefaRequest](./Models/TarefaRequest.md)
 - [TarefaResponse](./Models/TarefaResponse.md)
 - [TarefaUpdRequest](./Models/TarefaUpdRequest.md)
 - [TramiteNotaRequest](./Models/TramiteNotaRequest.md)
 - [TramiteResponse](./Models/TramiteResponse.md)
 - [UsuarioAlterarSenhaRequest](./Models/UsuarioAlterarSenhaRequest.md)
 - [UsuarioImagemResponse](./Models/UsuarioImagemResponse.md)
 - [UsuarioLoginRequest](./Models/UsuarioLoginRequest.md)
 - [UsuarioLoginResponse](./Models/UsuarioLoginResponse.md)
 - [UsuarioRequest](./Models/UsuarioRequest.md)
 - [UsuarioResponse](./Models/UsuarioResponse.md)
 - [UsuarioUpdRequest](./Models/UsuarioUpdRequest.md)


<a name="documentation-for-authorization"></a>
## Documentation for Authorization

<a name="Bearer"></a>
### Bearer

- **Type**: HTTP Bearer Token authentication (JWT)

