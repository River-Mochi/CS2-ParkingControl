// File: UI/src/extensions/districtPolicyFocus.tsx
// Purpose: Disables focus handling for Parking Control's expanded district policy row.

import type {
    ModuleRegistry,
    ModuleRegistryExtend,
} from "cs2/modding";

const POLICY_MODULE =
    "game-ui/game/components/policy/policy.tsx";

const POLICY_EXPORT = "Policy";

const PC_POLICY_ID = "PCDistrictParkingBan";

const PolicyFocusExtension: ModuleRegistryExtend = (Component) => {
    return (props: any) => {
        if (props?.policy?.id !== PC_POLICY_ID) {
            return <Component {...props} />;
        }

        return <Component {...props} disableFocus={true} />;
    };
};

export function registerDistrictPolicyFocusFix(
    moduleRegistry: ModuleRegistry
): void {
    moduleRegistry.extend(
        POLICY_MODULE,
        POLICY_EXPORT,
        PolicyFocusExtension
    );
}
