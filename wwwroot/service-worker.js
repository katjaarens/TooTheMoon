self.addEventListener('install', (e) => {
  console.log('[Service Worker] Install');
});

self.addEventListener('fetch', (e) => {
  // Leitet Anfragen normal weiter
  e.respondWith(fetch(e.request));
});