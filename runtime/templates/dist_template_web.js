if (!window.getCommonScript) {
    window.getCommonScript = (optVersion) => {
        if (!optVersion) optVersion = '%%%VERSION%%%';
        let ver = (optVersion || '').trim();
        let o = window.COMMON_SCRIPT_VERSIONS[ver];
        if (!o) throw new Error("CommonScript version not found: " + ver);
        return o;
    };
    window.COMMON_SCRIPT_VERSIONS = {};
}

{
    %%%COMMON_SCRIPT%%%
    window.COMMON_SCRIPT_VERSIONS['%%%VERSION%%%'] = CommonScript;
}
