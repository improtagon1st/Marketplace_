interface LegalPageProps {
  title: string
  intro: string
}

export function LegalPage({ title, intro }: LegalPageProps) {
  return (
    <div className="mx-auto w-full max-w-3xl rounded-3xl border border-slate-200 bg-white p-6 shadow-sm">
      <h1 className="text-2xl font-extrabold text-ink">{title}</h1>
      <p className="mt-2 text-sm text-slate-600">{intro}</p>

      <div className="mt-6 space-y-4 text-sm leading-6 text-slate-700">
        <section>
          <h2 className="font-bold text-ink">Оператор</h2>
          <p>Интернет-магазин Marketplace.</p>
        </section>
        <section>
          <h2 className="font-bold text-ink">Какие данные собираются</h2>
          <p>ФИО, email, номер телефона и адрес доставки.</p>
        </section>
        <section>
          <h2 className="font-bold text-ink">Цель сбора</h2>
          <p>Регистрация пользователя, обработка заказов и предоставление доступа к личному кабинету.</p>
        </section>
        <section>
          <h2 className="font-bold text-ink">Срок действия</h2>
          <p>С момента предоставления данных и до момента отзыва согласия пользователем.</p>
        </section>
      </div>
    </div>
  )
}
