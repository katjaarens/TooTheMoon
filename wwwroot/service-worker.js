self.addEventListener('install', (e) => {
  e.waitUntil(
    caches.open('wedding-admin-v1').then((cache) => {
      return cache.addAll([
        '/',
        '/Admin/AdminGuests',
        // Füge hier bei Bedarf weitere statische CSS/JS-Pfade hinzu
      ]);
    })
  );
});

self.addEventListener('fetch', (e) => {
  e.responsetWith(
    caches.match(e.request).then((response) => {
      return response || fetch(e.request);
    })
  );
});