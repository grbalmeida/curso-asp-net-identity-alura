using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        private UserManager<UsuarioAplicacao> _userManager;
        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextoOwin = HttpContext.GetOwinContext();
                    _userManager = contextoOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }

                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        private SignInManager<UsuarioAplicacao, string> _signInManager;
        public SignInManager<UsuarioAplicacao, string> SignInManager
        {
            get
            {
                if (_signInManager == null)
                {
                    var contextoOwin = HttpContext.GetOwinContext();
                    _signInManager = contextoOwin.GetUserManager<SignInManager<UsuarioAplicacao, string>>();
                }

                return _signInManager;
            }
            set
            {
                _signInManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                var contextoOwin = Request.GetOwinContext();
                return contextoOwin.Authentication;
            }
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {   
                var novoUsuario = new UsuarioAplicacao();

                novoUsuario.Email = modelo.Email;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                var usuario = await UserManager.FindByEmailAsync(modelo.Email);
                var usuarioJaExiste = usuario != null;

                if (usuarioJaExiste)
                    return View("AguardandoConfirmacao");

                var resultado = await UserManager.CreateAsync(novoUsuario, modelo.Senha);

                if (resultado.Succeeded)
                {
                    await EnviarEmailDeConfirmacaoAsync(novoUsuario);
                    return View("AguardandoConfirmacao");
                }
                else
                {
                    AdicionaErros(resultado);
                }
            }

            return View(modelo);
        }

        public async Task<ActionResult> ConfirmacaoEmail(string usuarioId, string token)
        {
            if (usuarioId == null || token == null)
                return View("Error");

            var resultado = await UserManager.ConfirmEmailAsync(usuarioId, token);

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");
            
            return View("Error");
        }

        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(ContaLoginViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario == null)
                    return SenhaOuUsuarioInvalidos();

                var signInResultado = await SignInManager.PasswordSignInAsync(
                    usuario.UserName,
                    modelo.Senha,
                    isPersistent: modelo.ContinuarLogado,
                    shouldLockout: true
                );

                // O argumento isPersistent indica se o cookie deve permanecer no navegador após o usuário fechar o navegador
                // O argumento shouldLockout indica se devemos bloquear o usuário de tentar se autenticar após determinado número de tentativas falhas

                switch (signInResultado)
                {
                    case SignInStatus.Success:
                        if (!usuario.EmailConfirmed)
                        {
                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                            return View("AguardandoConfirmacao");
                        }

                        return RedirectToAction("Index", "Home");
                    case SignInStatus.LockedOut:
                        var senhaCorreta = await UserManager.CheckPasswordAsync(usuario, modelo.Senha);

                        if (senhaCorreta)
                        {
                            ModelState.AddModelError("", "A conta está bloqueada!");
                            break;
                        }
                        else
                        {
                            return SenhaOuUsuarioInvalidos();
                        }
                    default:
                        return SenhaOuUsuarioInvalidos();
                }
            }

            return View(modelo);
        }

        public ActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> EsqueciSenha(ContaEsqueciSenhaViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                // Gerar o token de reset de senha
                // Gerar a url para o usuário
                // Vamos enviar esse email

                var usuario = await UserManager.FindByEmailAsync(modelo.Email);

                if (usuario != null)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(usuario.Id);

                    var linkDeCallback =
                        Url.Action(
                            nameof(ConfirmacaoAlteracaoSenha),
                            "Conta",
                            new { usuarioId = usuario.Id, token },
                            Request.Url.Scheme
                        );

                    await UserManager.SendEmailAsync(
                        usuario.Id,
                        "Fórum ByteBank - Alteração de senha",
                        $"Clique aqui {linkDeCallback} para alterar a sua senha!"
                    );
                }

                return View("EmailAlteracaoSenhaEnviado");
            }

            return View();
        }

        public ActionResult ConfirmacaoAlteracaoSenha(string usuarioId, string token)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Logoff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private ActionResult SenhaOuUsuarioInvalidos()
        {
            ModelState.AddModelError("", "Credenciais inválidas!");
            return View(nameof(Login));
        }

        private async Task EnviarEmailDeConfirmacaoAsync(UsuarioAplicacao usuario)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(usuario.Id);

            var linkDeCallback =
                Url.Action(
                    nameof(ConfirmacaoEmail),
                    "Conta",
                    new { usuarioId = usuario.Id, token },
                    Request.Url.Scheme
                );

            await UserManager.SendEmailAsync(
                usuario.Id,
                "Fórum ByteBank - Confirmação de Email",
                $"Bem vindo ao fórum ByteBank, clique aqui {linkDeCallback} para confirmar seu email!"
            );
        }

        private void AdicionaErros(IdentityResult resultado)
        {
            foreach (var erro in resultado.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }
    }
}