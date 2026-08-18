// File: UI/src/index.tsx
// Purpose: Registers Parking Control UI diagnostics.

import type { ModRegistrar } from "cs2/modding";
import { registerPolicyFocusDiagnostics } from "./diagnostics/policyFocusDiagnostics";

// Ensure the custom district icon is emitted to coui://ui-mods/images/.
import "../images/PC-DistrictParkingBan.svg";

const register: ModRegistrar = (moduleRegistry) => {
    registerPolicyFocusDiagnostics(
        moduleRegistry
    );
};

export default register;
