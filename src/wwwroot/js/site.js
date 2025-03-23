let mouseEvents = [];
        
document.addEventListener('mousemove', e => {
    mouseEvents.push({ 
        X: e.clientX, 
        Y: e.clientY, 
        T: Date.now() 
    });
});

function sendData() {
    console.log('Отправляемые данные:', mouseEvents); // <- Добавьте эту строку
    
    fetch('/Home/Save', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(mouseEvents)
    })
    .then(response => {
        console.log('Ответ сервера:', response.status);
        return response.text();
    })
    .catch(error => console.error('Ошибка:', error));
    
    mouseEvents = [];
}