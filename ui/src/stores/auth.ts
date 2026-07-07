import { persistentAtom } from '@nanostores/persistent';

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
}

// Full login/refresh logic arrives in Phase 2 — this just reserves the shape.
export const $authTokens = persistentAtom<AuthTokens | null>('auth-tokens', null, {
  encode: JSON.stringify,
  decode: JSON.parse,
});
