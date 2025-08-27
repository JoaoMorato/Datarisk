function bhaskara(a, b, c) {
    let d = Math.pow(b, 2) - (4 * a * c); 
    if (d < 0) return "Não possui raízes reais."; 
    return { 
        I: (-b + Math.sqrt(d)) / (2 * a), 
        II: (-b - Math.sqrt(d)) / (2 * a), 
    };
}

function fibonacci(n){
    return n < 1 ? 0 : (n == 1 ? 1 : (fibonacci(n-1) + fibonacci(n-2)));
}


function DadosVenda(data){
    let resp = {};
    for(let d of data){
        if(!resp[d.tag]){
            resp[d.tag] = {
                ValorTotal: d.valor,
                Quantidade: 1,
            }
            continue;
        }
        resp[d.tag].ValorTotal = parseFloat((resp[d.tag].ValorTotal + d.valor).toFixed(2));
        resp[d.tag].Quantidade++;
    }

    return resp;
}