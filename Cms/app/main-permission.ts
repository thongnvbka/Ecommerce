import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { PermissionModule } from './permission.module';

const platform = platformBrowserDynamic();
platform.bootstrapModule(PermissionModule);
