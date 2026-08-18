// File: UI/src/index.tsx
// Purpose: Registers Parking Control UI extensions.

import type { ModRegistrar } from "cs2/modding";
import { registerDistrictPolicyFocusProbe } from "./extensions/districtPolicyFocus";

// Ensure the custom district icon is emitted to coui://ui-mods/images/.
import "../images/PC-DistrictParkingBan.svg";

const register: ModRegistrar = (moduleRegistry) => {
    registerDistrictPolicyFocusProbe(moduleRegistry);
};

export default register;
