// File: UI/src/extensions/districtPolicyFocus.tsx
// Purpose: Prevents invalid focus registration in Parking Control's expanded district policy row.

import { FocusDisabled } from "cs2/input";

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

        return (
            <FocusDisabled>
                <Component {...props} />
            </FocusDisabled>
        );
    };
};

export function registerDistrictPolicyFocusFix(
    moduleRegistry: ModuleRegistry
): void {
    try {
        moduleRegistry.extend(
            POLICY_MODULE,
            POLICY_EXPORT,
            PolicyFocusExtension
        );
    } catch (error) {
        console.error(
            `[ParkingControl][UI] extend failed for ${POLICY_MODULE}#${POLICY_EXPORT}`,
            error
        );
    }
}
