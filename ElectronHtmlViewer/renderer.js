onload = () => {
    document.querySelector('#btn-next').addEventListener('click', _ => {
      window.api.next()
    })
    document.querySelector('#btn-prev').addEventListener('click', _ => {
      window.api.prev()
    })
  }