# LyraSEOOverlay Guidance - Sprint 1003

## SEO Meta Injection Standards

### Required Meta Tags for All Pages:
1. **Title Tag**: `<title>@ViewData["Title"] - Service Atlanta</title>`
2. **Description**: `<meta name="description" content="@ViewData["MetaDescription"]" />`
3. **Keywords**: `<meta name="keywords" content="@ViewData["Keywords"]" />`
4. **Robots**: `<meta name="robots" content="@ViewData["Robots"]" />`
5. **Canonical Link**: `<link rel="canonical" href="@Context.Request.GetDisplayUrl()" />`

### OpenGraph Tags:
1. **OG Title**: `<meta property="og:title" content="@ViewData["Title"]" />`
2. **OG Description**: `<meta property="og:description" content="@ViewData["MetaDescription"]" />`
3. **OG Type**: `<meta property="og:type" content="website" />`
4. **OG URL**: `<meta property="og:url" content="@Context.Request.GetDisplayUrl()" />`
5. **OG Image**: `<meta property="og:image" content="@(ViewData["OgImage"] ?? "/images/service-atlanta-og.jpg")" />`

### Layout Reference Check:
- Ensure each page has proper Layout reference: `Layout = "~/Pages/Shared/_Layout.cshtml";`

### Implementation:
- Add SEO metadata in page @{} block
- Use [FixItFredComment:Sprint1003 - SEO Meta Injection] for all changes