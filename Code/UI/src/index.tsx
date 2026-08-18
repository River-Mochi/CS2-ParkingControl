// File: UI/src/index.tsx
// Purpose: Registers Parking Control UI extensions.

import type { ModRegistrar } from "cs2/modding";
import { registerDistrictPolicyFocusFix } from "./extensions/districtPolicyFocus";

// Ensure the custom district icon is emitted to coui://ui-mods/images/.
import "../images/PC-DistrictParkingBan.svg";

const register: ModRegistrar = (moduleRegistry) => {
    registerDistrictPolicyFocusFix(moduleRegistry);
};

export default register;
