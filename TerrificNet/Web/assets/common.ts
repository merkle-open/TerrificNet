 class Common {
     static parseQueryString(href: string): { [id: string]: string; } {
         var vars: { [id: string]: string; } = {}, hash;
         var hashes = href.slice(href.indexOf('?') + 1).split('&');
         for (var i = 0; i < hashes.length; i++) {
             hash = hashes[i].split('=');
             //vars.push(hash[0]);
             vars[hash[0]] = hash[1];
         }
         return vars;
     }
 }