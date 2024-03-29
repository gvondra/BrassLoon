import { Component, OnDestroy, OnInit  } from '@angular/core';
import { Router } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { HttpClientUtilService } from './http-client-util.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styles: []
})
export class AppComponent implements OnInit, OnDestroy {
    title = 'Brass Loon';
    UserImageSource: string = null;
    IsLoggedIn: boolean = false;

    constructor(public oidcSecurityService: OidcSecurityService, 
        private router: Router,
        private httpClientUtilService: HttpClientUtilService) { }

    ngOnInit(): void {
        console.log("Start app component init")
        this.IsLoggedIn = false;
        this.UserImageSource = null;
        this.oidcSecurityService
        .checkAuth()
        .subscribe((isAuthenticated) => {
            console.log(`app init is authenticated ${isAuthenticated.isAuthenticated}`)
            /*if (!isAuthenticated.isAuthenticated) {
                this.UserImageSource = null;
                this.httpClientUtilService.DropCache();
                if (!window.location.pathname.endsWith('autologin')) {
                    this.write('redirect', window.location.pathname);
                    this.router.navigate(['/autologin']);
                }
            }
            if (isAuthenticated.isAuthenticated) {
                this.navigateToStoredEndpoint();
            }*/ 
        });
        this.oidcSecurityService.isAuthenticated$.subscribe(isAuthenticated => {
            this.IsLoggedIn = isAuthenticated.isAuthenticated;
            if (isAuthenticated.isAuthenticated) {
                this.SetUserImageSource();
            }
            else {
                this.UserImageSource = null;
            }
        });
    }

    ngOnDestroy() : void {}

    Login() {
        this.oidcSecurityService.authorize();
    }

    private navigateToStoredEndpoint() {
        let path: string = this.read('redirect');
        this.remove('redirect');
        if (path)
        {
            if (this.router.url === path) {
                return;
            }
    
            if (path.toString().includes('/unauthorized')) {
                this.router.navigate(['/']);
            } else {
                if (!path.startsWith('/')) {
                    path = '/' + path;
                }
                window.location.href = path;
            }
        }
    }

    private read(key: string): any {
        const data = localStorage.getItem(key);
        if (data != null) {
            return JSON.parse(data);
        }

        return;
    }

    private write(key: string, value: any): void {
        localStorage.setItem(key, JSON.stringify(value));
    }

    private remove(key: string){
        localStorage.removeItem(key);
    }

    NavigateHome()
    {
        this.router.navigate(["/"]);
    }

    private SetUserImageSource() {
        this.oidcSecurityService.getPayloadFromIdToken()
        .subscribe(tkn => {
            if (tkn && tkn.picture) {
                this.UserImageSource = tkn.picture;
            }        
            else {
                this.UserImageSource = null;
            }
        });        
    }
}
