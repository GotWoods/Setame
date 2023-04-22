export class SettingGridData {
    constructor(environments =[], settings = []) {
        this.environments = environments;
        this.settings = settings;
      }
}

export class SettingGridItem {
    constructor(settingName, settings) {
        this.settingName = settingName;
        this.settings = settings;
      }
}

