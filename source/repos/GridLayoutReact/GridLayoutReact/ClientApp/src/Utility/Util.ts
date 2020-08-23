import axios from "axios";

export  class Utilities {


    public capitalizeFLetter(inputValue: string): string {
        return inputValue && inputValue[0].toUpperCase() + inputValue.slice(1);
    }


    public async getDataFromDB(url: string) {
        let data;
        await axios.get(url)
            .then(response => {
                data = response;
                console.log(response);
            })
            .catch(error => {
                console.log(error)
            });
        return data;
    }

}