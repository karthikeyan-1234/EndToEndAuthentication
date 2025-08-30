import Keycloak from 'keycloak-js';
import { KeycloakService } from 'keycloak-angular';

export function initializeKeycloak(keycloak: KeycloakService): () => Promise<void> {
  return (): Promise<void> =>
    keycloak.init({
      config: {
        url: 'http://192.168.1.150:8443',
        realm: 'master',
        clientId: 'angular-app',
      },
      initOptions: {
        onLoad: 'login-required', //check-sso
        //silentCheckSsoRedirectUri: window.location.origin + '/assets/silent-check-sso.html'
      },
    }).then(() => {});
}