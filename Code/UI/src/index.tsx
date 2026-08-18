// File: UI/src/index.tsx
// Purpose: Locates vanilla policy UI modules needed for the Parking Control focus fix.

import type { ModRegistrar } from "cs2/modding";

const register: ModRegistrar = (moduleRegistry) => {
  const matches = moduleRegistry.find(/polic/i);

  for (const [path, ...exports] of matches) {
    console.log(
      `[ParkingControl][UI] ${path} :: ${exports.join(", ")}`
    );
  }
};

export default register;
