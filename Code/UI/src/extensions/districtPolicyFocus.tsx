// File: UI/src/extensions/districtPolicyFocus.tsx
// Purpose: Finds the vanilla PanelSectionRow module used by the CS2 UI.

import type { ModuleRegistry } from "cs2/modding";

export function registerDistrictPolicyFocusProbe(
    moduleRegistry: ModuleRegistry
): void {
    const matches = moduleRegistry.find(
        /PanelSectionRow|panel-section-row/i
    );

    if (matches.length === 0) {
        console.warn(
            "[ParkingControl][UI][PanelSectionRow] no matching module found"
        );
        return;
    }

    for (const [path, ...exports] of matches) {
        console.log(
            `[ParkingControl][UI][PanelSectionRow] ${path} :: ${exports.join(", ")}`
        );
    }
}
