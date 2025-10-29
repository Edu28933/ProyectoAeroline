(() => {
  const mascots = document.getElementById('mascots');
  const pupils  = mascots.querySelectorAll('.pupil');
  const email   = document.getElementById('email');
  const pwd     = document.getElementById('pwd');
  const btnEye  = document.getElementById('btnToggle');
  const form    = document.getElementById('loginForm');
  const MAX = 8;

  // ===== 1) Intro: scatter -> drop -> stack =====
  mascots.className = 'pose-scatter';
  setTimeout(()=> mascots.classList.add('pose-drop'), 500);
  setTimeout(()=>{
    mascots.classList.remove('pose-scatter','pose-drop');
    mascots.classList.add('pose-stack');
  }, 1200);

  // ===== 2) OJOS siguen el mouse =====
  function trackTo(x,y){
    const rect = mascots.getBoundingClientRect();
    const mx = x - rect.left, my = y - rect.top;

    pupils.forEach(p=>{
      const g  = p.closest('.eyes') || p.parentElement;
      const gr = g.getBoundingClientRect();
      const cx = gr.left + gr.width/2 - rect.left;
      const cy = gr.top  + gr.height/2 - rect.top;

      const dx = mx - cx, dy = my - cy;
      const d  = Math.max(1, Math.hypot(dx,dy));
      p.style.transform = `translate(${(dx/d)*MAX}px, ${(dy/d)*MAX}px)`;
    });
  }
  window.addEventListener('mousemove', e => trackTo(e.clientX, e.clientY));

  // Miran al enfocar
  function lookAt(el){
    const r = el.getBoundingClientRect();
    trackTo(r.left + r.width/2, r.top + r.height/2);
  }
  email?.addEventListener('focus', ()=> lookAt(email));
  pwd  ?.addEventListener('focus', ()=> lookAt(pwd));

  // ===== 3) Mostrar/ocultar contraseña (volteo) =====
  btnEye?.addEventListener('click', ()=>{
    const show = pwd.type === 'password';
    pwd.type = show ? 'text' : 'password';
    btnEye.innerHTML = show ? '<i class="bi bi-eye-slash"></i>' : '<i class="bi bi-eye"></i>';
    mascots.classList.toggle('peek', show);
  });

  // ===== 4) Validación DEMO (cámbiala por tu backend si quieres) =====
  const VALID_EMAIL = 'anna@gmail.com';
  const VALID_PASS  = '77709555A5!';

  form?.addEventListener('submit', (e)=>{
    e.preventDefault();
    const ok = email.value.trim() === VALID_EMAIL && pwd.value === VALID_PASS;

    mascots.classList.remove('happy','sad','peek');

    if(!ok){
      mascots.classList.add('sad','shake');
      mascots.addEventListener('animationend', ()=> mascots.classList.remove('shake'), {once:true});
      return;
    }

    mascots.classList.add('happy');
    setTimeout(()=> window.location.href = "/home", 600);
  });
})();
