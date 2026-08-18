// File: UI/src/extensions/districtPolicyFocus.tsx
// Purpose: Disables focus handling only for Parking Control's expanded district policy row.

import {
    createContext,
    useContext,
} from "react";

import type {
    ModuleRegistry,
    ModuleRegistryExtend,
} from "cs2/modding";

import { VANILLA_COMPONENT_MODULES } from "../utils/vanilla/components";

const POLICY_MODULE =
    "game-ui/game/components/policy/policy.tsx";

const POLICY_EXPORT = "Policy";

const PC_POLICY_ID = "PCDistrictParkingBan";

const PcPolicyContext = createContext(false);

const PolicyScopeExtension: ModuleRegistryExtend = (Component) => {
    return (props: any) => {
        if (props?.policy?.id !== PC_POLICY_ID) {
            return <Component {...props} />;
        }

        return (
            <PcPolicyContext.Provider value={true}>
                <Component {...props} />
            </PcPolicyContext.Provider>
        );
    };
};

const InfoRowFocusExtension: ModuleRegistryExtend = (Component) => {
    return (props: any) => {
        const isPcPolicy = useContext(PcPolicyContext);

        if (!isPcPolicy) {
            return <Component {...props} />;
        }

        return <Component {...props} disableFocus={true} />;
    };
};

function extendSafe(
    moduleRegistry: ModuleRegistry,
    modulePath: string,
    exportId: string,
    extension: ModuleRegistryExtend
): void {
    try {
        moduleRegistry.extend(
            modulePath,
            exportId,
            extension
        );
    } catch (error) {
        console.error(
            `[ParkingControl][UI] extend failed for ${modulePath}#${exportId}`,
            error
        );
    }
}

export function registerDistrictPolicyFocusFix(
    moduleRegistry: ModuleRegistry
): void {
    extendSafe(
        moduleRegistry,
        POLICY_MODULE,
        POLICY_EXPORT,
        PolicyScopeExtension
    );

    const [infoRowModule, infoRowExport] =
        VANILLA_COMPONENT_MODULES.InfoRow;

    extendSafe(
        moduleRegistry,
        infoRowModule,
        infoRowExport,
        InfoRowFocusExtension
    );
}
