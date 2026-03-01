# ProductBuilder UI

React + TypeScript frontend for Insurance Product Builder.

## Stack
- React 19
- TypeScript
- Vite 7
- Tailwind CSS 3
- TanStack Query
- Axios

## Run
```bash
npm install
npm run dev
```

- App: `http://localhost:5173`
- API base: `http://localhost:5018/api`

## Environment
Create/update `.env`:

```env
VITE_API_BASE_URL=http://localhost:5018/api
```

## Scripts
- `npm run dev` - start dev server
- `npm run build` - type-check + production build
- `npm run test` - vitest watch mode
- `npm run test:run` - vitest single run

## Notes
- Uses shared Axios client in `src/api/client.ts` with JWT attach + refresh-on-401.
- Broker/Underwriter selection is supported in quote create/edit flows.
- Underwriter/Broker create forms only show eligible users (active + matching role + not already assigned).
