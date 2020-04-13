import {Pipe} from "@angular/core";

import {CommonService} from '../services/common.service'

declare var _;

@Pipe({
    name: "search"
})

export class PermissionDetailPipe {
    transform(values, term) {
        const keyword = CommonService.stripVietnameseChars(term).toLowerCase();

        if (_.trim(keyword) === "")
            return values;

        let items = values.filter(i => {
            const pageName = CommonService.stripVietnameseChars(i.pageName).toLowerCase();
            return pageName.indexOf(keyword) >= 0;
        });

        let moduleIdFist;
        _.forEach(items, (item) => {
            item.firstRecord = moduleIdFist !== item.moduleId;
            item.pathName = `${item.appName} / ${item.moduleName}`;

            moduleIdFist = item.moduleId;
        });
        return items;
    }
}